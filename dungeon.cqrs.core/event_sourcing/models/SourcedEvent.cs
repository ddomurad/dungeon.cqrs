using dungeon.cqrs.core.events;

namespace dungeon.cqrs.core.event_sourcing.models
{
    public class SourcedEvent : IEvent
    {
        public string EventId { get; set; }
        public SourcedEventMetadata Metadata { get; set; }
    }
}