using System.Threading.Tasks;
using dungeon.cqrs.core.commands;
using dungeon.cqrs.core.event_sourcing.models;
using dungeon.cqrs.core.events;
using dungeon.cqrs.test.Models;

namespace dungeon.cqrs.test
{
    public class UserEventHandler :
        IEventHandler<AggregateCreatedEvent>,
        IEventHandler<AggregateRemovedEvent>,
        IEventHandler<LoginReservedEvent>
    {
        public Task Handle(AggregateCreatedEvent e, ICommand c)
        {
            //System.Console.WriteLine("Obj created: " + e.Metadata.AggregateType);
            return Task.CompletedTask;
        }

        public Task Handle(LoginReservedEvent e, ICommand c)
        {
            //System.Console.WriteLine("Login reserver: " + e.Metadata.AggregateId + " for " + e.UserId);
            return Task.CompletedTask;
        }

        public Task Handle(AggregateRemovedEvent e, ICommand c)
        {
            //System.Console.WriteLine("Obj removed: " + e.Metadata.AggregateType);
            return Task.CompletedTask;
        }
    }
}