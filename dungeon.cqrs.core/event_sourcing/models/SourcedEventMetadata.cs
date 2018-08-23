using System;

namespace dungeon.cqrs.core.event_sourcing.models
{
    public class SourcedEventMetadata {
        public string AggregateId { get; set; }
        public Guid CommandId { get; set; }
        public ulong Version { get; set; }
        public string AggregateType { get; set; }

        public DateTime Date { get; set; }
    }
}