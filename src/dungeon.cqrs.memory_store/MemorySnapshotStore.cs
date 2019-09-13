using System.Collections.Generic;
using System.Threading.Tasks;
using dungeon.cqrs.core.event_sourcing;
using dungeon.cqrs.core.event_sourcing.models;

namespace dungeon.cqrs.memory_store
{
    public class MemorySnapshotStore : ISnapshotStore
    {
        private readonly object locker = new object();
        public readonly Dictionary<string, Memento> Store = new Dictionary<string, Memento>();

        public Task<Memento> Read(string aggregateId)
        {
            lock (locker)
            {
                if (Store.ContainsKey(aggregateId))
                    return Task.FromResult(Store[aggregateId]);

                return Task.FromResult((Memento)null);
            }
        }

        public Task<ulong> ReadVersion(string aggregateId)
        {
            lock (locker)
            {
                if (Store.ContainsKey(aggregateId))
                    return Task.FromResult(Store[aggregateId].Metadata.Version);

                return Task.FromResult((ulong)0);
            }
        }

        public Task Write(Memento memento)
        {
            lock (locker)
            {
                if (!Store.ContainsKey(memento.Metadata.AggregateId))
                    Store.Add(memento.Metadata.AggregateId, memento);
                else
                    Store[memento.Metadata.AggregateId] = memento;
            }

            return Task.CompletedTask;
        }
    }
}