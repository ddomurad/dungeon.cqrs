using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dungeon.cqrs.core.event_sourcing;
using dungeon.cqrs.core.event_sourcing.models;

namespace dungeon.cqrs.memory_store {
    public class MemoryEventStore : IEventStore {
        private readonly object locker = new object ();

        public readonly Dictionary<string, List<SourcedEvent>> Events = new Dictionary<string, List<SourcedEvent>> ();

        public readonly List<SourcedCommand> Commands = new List<SourcedCommand> ();

        public Task SaveEvents (IEnumerable<SourcedEvent> events) {
            lock (locker) {
                events.ToList ().ForEach (e => {
                    if (!Events.ContainsKey (e.Metadata.AggregateId))
                        Events.Add (e.Metadata.AggregateId, new List<SourcedEvent> ());

                    Events[e.Metadata.AggregateId].Add (e);
                });

                return Task.CompletedTask;
            }
        }

        public Task<IEnumerable<SourcedEvent>> ReadEvents (string aggregateId) {
            lock (locker) {
                if (Events.ContainsKey (aggregateId))
                    return Task.FromResult (Events[aggregateId].OrderBy (e => e.Metadata.Version).AsEnumerable ());

                return Task.FromResult (new List<SourcedEvent> ().AsEnumerable ());
            }
        }

        public Task<IEnumerable<SourcedEvent>> ReadEvents (string aggregateId, ulong fromVersion) {
            lock (locker) {
                if (Events.ContainsKey (aggregateId))
                    return Task.FromResult (
                        Events[aggregateId].Where (e => e.Metadata.Version > fromVersion).OrderBy (e => e.Metadata.Version).AsEnumerable ());
                return Task.FromResult (new List<SourcedEvent> ().AsEnumerable ());
            }
        }

        public Task<IEnumerable<SourcedEvent>> ReadEvents (string aggregateId, Guid cmdId) {
            lock (locker) {
                if (Events.ContainsKey (aggregateId))
                    return Task.FromResult (
                        Events[aggregateId].Where (e => e.Metadata.CommandId == cmdId).OrderBy (e => e.Metadata.Version).AsEnumerable ());
                return Task.FromResult (new List<SourcedEvent> ().AsEnumerable ());
            }
        }

        public Task<IEnumerable<SourcedEvent>> ReadAllEvents (DateTime? replayTill) {
            lock (locker) {
                return Task.FromResult (Events.Values.SelectMany (e => e)
                    .Where (e => e.Metadata.Date <= replayTill)
                    .OrderBy (e => e.Metadata.Version)
                    .AsEnumerable ());
            }
        }

        public Task<IEnumerable<SourcedEvent>> ReadAllEvents (string aggregateId, DateTime? replayTill) {
            lock (locker) {
                return Task.FromResult (Events.Values.SelectMany (e => e)
                    .Where (e => e.Metadata.AggregateId == aggregateId && e.Metadata.Date <= replayTill)
                    .OrderBy (e => e.Metadata.Version)
                    .AsEnumerable ());
            }
        }

        public Task<SourcedEvent> ReadLastEvent (string aggregateId) {
            lock (locker) {
                if (Events.ContainsKey (aggregateId))
                    return Task.FromResult (
                        Events[aggregateId].OrderBy (e => e.Metadata.Version).LastOrDefault ());

                return Task.FromResult ((SourcedEvent) null);
            }
        }

        public Task SaveCommand (SourcedCommand command) {
            lock (locker) {
                Commands.Add (command);
                return Task.CompletedTask;
            }
        }
    }
}