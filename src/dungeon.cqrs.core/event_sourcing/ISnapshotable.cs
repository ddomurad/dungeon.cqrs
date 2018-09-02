using dungeon.cqrs.core.event_sourcing.models;

namespace dungeon.cqrs.core.event_sourcing
{
    public interface ISnapshotable
    {}

    public interface ISnapshotable<TMemento> : ISnapshotable where TMemento : Memento
    {
        TMemento TakeSnapshot();
        void RestoreSnapshot(TMemento memenot);
    }
}