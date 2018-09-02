using dungeon.cqrs.core.event_sourcing.models;

namespace dungeon.cqrs.core.event_sourcing
{
    public interface ISnapshotConfiguration
    {
        void AddSnapshotConfig<T>(SnapshotConfig config);
        SnapshotConfig GetConfiguration(IAggregateRoot snapshotable);
        void SetDefaultSnapshotConfig(SnapshotConfig config);
    }
}