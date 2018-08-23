using System.Threading.Tasks;

namespace dungeon.cqrs.core.commands
{
    public interface ICommandHandler { }

    public interface ICommandHandler<in TCommand> : ICommandHandler
    where TCommand : ICommand {
        Task Handle (TCommand command);
    }

    public interface ICommandHandler<TReturn, in TCommand> : ICommandHandler
    where TCommand : ICommand {
        Task<TReturn> Handle (TCommand command);
    }


    }