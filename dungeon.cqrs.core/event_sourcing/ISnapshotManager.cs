using System.Threading.Tasks;
using dungeon.cqrs.core.event_sourcing.models;

namespace dungeon.cqrs.core.event_sourcing {
    public interface ISnapshotManager {
        Task TryRestore (IAggregateRoot aggregate, string aggregateId);
        Task TakeSnapshotSync (IAggregateRoot aggregate);
        Task<bool> ShouldTakeSnapshot (IAggregateRoot aggregate);
    }
}