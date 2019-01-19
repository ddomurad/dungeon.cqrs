using System.Threading.Tasks;
using dungeon.cqrs.core.event_sourcing;

namespace dungeon.cqrs.event_sourcing
{
    public class NullSnapshotManager : ISnapshotManager
    {
        public Task<bool> ShouldTakeSnapshot(IAggregateRoot aggregate)
        {
            return Task.FromResult(false);
        }

        public Task TakeSnapshotSync(IAggregateRoot aggregate)
        {
            return Task.CompletedTask;
        }

        public Task TryRestore(IAggregateRoot aggregate, string aggregateId)
        {
            return Task.CompletedTask;
        }
    }
}