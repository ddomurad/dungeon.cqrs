using System.Collections.Generic;
using dungeon.cqrs.core.event_sourcing.models;

namespace dungeon.cqrs.core.event_sourcing
{
    public interface IAggregateRoot
    {
        AggregateMetadata Metadata { get; }
        void RestoreEvent(SourcedEvent e);
        IEnumerable<SourcedEvent> NotCommitedChanges();
        void CommitChanges();
        void CreateNew(string id);
        void Delete();
    }
}