using System.Threading.Tasks;
using dungeon.cqrs.core.commands;

namespace dungeon.cqrs.core.events
{
    public interface IEventSender
    {
        Task Send<TE>(TE e) where TE : IEvent;
        Task Send<TE, TC>(TE e, TC c)
            where TE : IEvent
            where TC : ICommand;
    }
}