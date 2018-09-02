
using dungeon.cqrs.core.event_sourcing.models;

namespace dungeon.cqrs.test.Models
{
    public class LoginReservedEvent : SourcedEvent {
        public string UserId {get; set;}
    }
}