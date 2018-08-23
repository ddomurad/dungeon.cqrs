using System.Threading.Tasks;
using dungeon.cqrs.core.event_sourcing.models;

namespace dungeon.cqrs.core.event_sourcing {
    public interface ISnapshotStore {
        Task<Memento> Read (string aggregateId);
        Task<ulong> ReadVersion (string aggregateId);
        Task Write (Memento memento);
    }
}