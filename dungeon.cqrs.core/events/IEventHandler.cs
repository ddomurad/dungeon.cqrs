using System.Threading.Tasks;
using dungeon.cqrs.core.commands;

namespace dungeon.cqrs.core.events
{
    public interface IEventHandler { }

    public interface IEventHandler<in TEvent> : IEventHandler
        where TEvent : IEvent
    {
        Task Handle(TEvent e, ICommand c);
    }

}