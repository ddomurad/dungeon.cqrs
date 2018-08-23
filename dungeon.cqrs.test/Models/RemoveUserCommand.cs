using dungeon.cqrs.core.event_sourcing.models;

namespace dungeon.cqrs.test.Models
{
    public class RemoveUserCommand : SourcedCommand {
        public string Login {get; set;}
    }
}