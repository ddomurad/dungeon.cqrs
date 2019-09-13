using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dungeon.cqrs.core.commands;
using dungeon.cqrs.core.event_sourcing;
using dungeon.cqrs.core.event_sourcing.models;
using dungeon.cqrs.core.events;
using dungeon.cqrs.core.exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace dungeon.cqrs.event_sourcing
{
    public class EventSource : IEventSource
    {
        private readonly List<IAggregateRoot> commitedEventStreams = new List<IAggregateRoot>();
        private readonly IEventStore eventStore;
        private readonly ISnapshotManager snapshotManager;
        private readonly IEventSender eventSender;
        private readonly IServiceProvider serviceProvider;

        public EventSource(IEventSender eventSender, IEventStore eventStore, ISnapshotManager snapshotManager, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.eventSender = eventSender;
            this.eventStore = eventStore;
            this.snapshotManager = snapshotManager;
        }

        public async Task PushChanges(SourcedCommand command)
        {
            var cmdId = command.CommandId;

            foreach (var aggregate in commitedEventStreams)
                UpdateStream(aggregate, cmdId);

            // foreach (var aggregate in commitedEventStreams)
            //     await ValidateStream(aggregate);

            await SaveStream(commitedEventStreams);

            foreach (var aggregate in commitedEventStreams)
                UpdateAggregateVersion(aggregate);

            await eventStore.SaveCommand(command);

            foreach (var aggregate in commitedEventStreams)
                await SendEvents(aggregate, command);

            foreach (var aggregate in commitedEventStreams)
                aggregate.CommitChanges();

            if (snapshotManager != null)
                foreach (var aggregate in commitedEventStreams)
                    await TakeSnapshot(aggregate);

            commitedEventStreams.Clear();
        }

        private static void UpdateStream(IAggregateRoot stream, Guid commandId)
        {
            var notCommited = stream.NotCommitedChanges().ToList();
            if (!notCommited.Any())
                return;

            var versionCnt = stream.Metadata.Version;
            foreach (var bEvent in notCommited)
            {
                var aggregateType = stream.GetType().FullName;
                var new_version = ++versionCnt;

                bEvent.EventId = $"{stream.Metadata.AggregateId}_{new_version}";
                bEvent.Metadata = new SourcedEventMetadata
                {
                    Version = new_version,
                    CommandId = commandId,
                    AggregateId = stream.Metadata.AggregateId,
                    AggregateType = aggregateType,
                    Date = DateTime.UtcNow
                };
            }
        }

        // private async Task ValidateStream(IAggregateRoot stream)
        // {
        //     var oldVersion = await eventStore.ReadLastEvent(stream.Metadata.AggregateId);

        //     if (oldVersion == null)
        //         return;

        //     if (oldVersion.Metadata.Version != stream.Metadata.Version)
        //     {
        //         var errMsg = $"ValidateStream: Can't commit aggregate changes, because of version mismatch. Aggregate id: {stream.Metadata.AggregateId}. Old Version: {oldVersion.Metadata.Version}. New Version: {stream.Metadata.Version}";
        //         serviceProvider.GetService<ILoggerWrapper>()?.Warning(errMsg);
        //         throw new DungeonConcurrencyException(errMsg);
        //     }
        // }

        private async Task SaveStream(IEnumerable<IAggregateRoot> aggregateRoots)
        {
            var notCommited = aggregateRoots.Select(agg => agg.NotCommitedChanges()).SelectMany(_ => _).ToList();
            if (!notCommited.Any())
                return;

            await eventStore.SaveEvents(notCommited);
        }

        private static void UpdateAggregateVersion(IAggregateRoot stream)
        {
            var notCommited = stream.NotCommitedChanges().ToList();
            if (!notCommited.Any())
                return;

            stream.Metadata.Version = notCommited.Last().Metadata.Version;
        }

        private async Task SendEvents(IAggregateRoot stream, ICommand command)
        {
            var notCommited = stream.NotCommitedChanges().ToList();
            if (!notCommited.Any())
                return;

            if (eventSender != null)
            {
                foreach (var se in notCommited)
                    await eventSender.Send(se, command);
            }
        }

        public async Task<T> Pull<T>(string aggregateId) where T : class, IAggregateRoot
        {
            T aggregate = await RawPull<T>(aggregateId);

            if (aggregate.Metadata.Exists == false)
                return null;

            CommitAggregate(aggregate);
            return aggregate;
        }

        public async Task<T> CreateNew<T>(string id) where T : class, IAggregateRoot
        {
            var aggregate = await this.RawPull<T>(id);

            if (aggregate.Metadata.Exists)
            {
                var errMsg = $"CreateNew: Cant create aggregate. Aggregate already exists '{id}'.";
                serviceProvider.GetService<ILoggerFactory>()?.CreateLogger<EventSource>()?.LogWarning(errMsg);
                throw new DungeonAggregateAlreadyExistsException(errMsg);
            }

            aggregate.CreateNew(id);
            CommitAggregate(aggregate);
            return (T)aggregate;
        }

        private void CommitAggregate(IAggregateRoot stream)
        {
            if (!commitedEventStreams.Contains(stream))
                commitedEventStreams.Add(stream);
        }

        private async Task<T> RawPull<T>(string aggregateId) where T : IAggregateRoot
        {
            var aggregateType = typeof(T);
            if (aggregateType.GetConstructors().Count() > 1)
            {
                var errMsg = $"RawPull: Aggregate '{aggregateType.FullName}' has to many constructors.";
                serviceProvider.GetService<ILoggerFactory>()?.CreateLogger<EventSource>()?.LogError(errMsg);
                throw new DungeonInvalidConstructorException(
                    errMsg);
            }

            object[] ctorArgs = { };
            var ctorDef = aggregateType.GetConstructors().FirstOrDefault();

            if (ctorDef != null)
                ctorArgs = ctorDef.GetParameters().Select(p => serviceProvider.GetService(p.ParameterType)).ToArray();

            var aggregate = (T)Activator.CreateInstance(aggregateType, ctorArgs);

            if (snapshotManager != null)
                await snapshotManager.TryRestore(aggregate, aggregateId);

            var events = await eventStore.ReadEvents(aggregateId, aggregate.Metadata.Version);

            foreach (var e in events)
                aggregate.RestoreEvent(e);
            return aggregate;
        }

        private async Task TakeSnapshot(IAggregateRoot aggregate)
        {
            if (!(await snapshotManager.ShouldTakeSnapshot(aggregate)))
                return;

            await snapshotManager.TakeSnapshotSync(aggregate);
        }
    }
}