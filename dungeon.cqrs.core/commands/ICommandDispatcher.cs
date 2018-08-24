using System.Threading.Tasks;

namespace dungeon.cqrs.core.commands
{
    public interface ICommandDispatcher
    {
        Task DispatchAbstract(ICommand command, int concurencyRetry = 0);
        Task<TR> DispatchAbstract<TR>(ICommand command, int concurencyRetry = 0);
        Task Dispatch<TC>(TC command, int concurencyRetry = 0) where TC : ICommand;
        Task<TR> Dispatch<TR, TC>(TC command, int concurencyRetry = 0) where TC : ICommand;
    }
}