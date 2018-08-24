using System.Threading.Tasks;
using dungeon.cqrs.core.event_sourcing;
using dungeon.cqrs.core.event_sourcing.models;

namespace dungeon.cqrs.event_sourcing
{
    public class SnapshotManager : ISnapshotManager
    {
        private readonly ISnapshotStore snapshotStore;
        private readonly ISnapshotConfiguration snapshotConfiguration;

        public SnapshotManager(ISnapshotStore snapshotStore, ISnapshotConfiguration snapshotConfiguration)
        {
            this.snapshotStore = snapshotStore;
            this.snapshotConfiguration = snapshotConfiguration;
        }

        public async Task TryRestore(IAggregateRoot aggregate, string aggregateId)
        {
            if (!(aggregate is ISnapshotable))
                return;

            var snapshot = await snapshotStore.Read(aggregateId);
            if (snapshot != null)
                RestoreSnapshot(aggregate, snapshot);
        }

        public async Task TakeSnapshotSync(IAggregateRoot aggregate)
        {
            if (!(aggregate is ISnapshotable))
                return;

            var takeSnapshotMethod = aggregate.GetType().GetMethod("TakeSnapshot");
            var memento = (Memento)takeSnapshotMethod.Invoke(aggregate, new object[] { });

            await snapshotStore.Write(memento);
        }

        public async Task<bool> ShouldTakeSnapshot(IAggregateRoot aggregate)
        {
            if (!(aggregate is ISnapshotable))
                return false;

            var config = snapshotConfiguration.GetConfiguration(aggregate);
            if (config == null)
                return false;

            var storedSnapshotVersion = await snapshotStore.ReadVersion(aggregate.Metadata.AggregateId);
            return aggregate.Metadata.Version - storedSnapshotVersion >= config.TakeSnapshotWhenVersionDiferrenceIsBiggerThanOrEqual;
        }

        private void RestoreSnapshot(IAggregateRoot aggregate, Memento memento)
        {
            var restoreSnapshotMethod = aggregate.GetType().GetMethod("RestoreSnapshot");
            restoreSnapshotMethod.Invoke(aggregate, new object[] { memento });
        }

    }
}