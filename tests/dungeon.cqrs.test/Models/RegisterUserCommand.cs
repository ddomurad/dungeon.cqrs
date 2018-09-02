
using dungeon.cqrs.core.event_sourcing.models;

namespace dungeon.cqrs.test.Models
{
    public class RegisterUserCommand : SourcedCommand {
        public string Login { get; set; }
    }
}