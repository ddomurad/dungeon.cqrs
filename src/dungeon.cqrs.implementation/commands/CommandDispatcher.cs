using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using dungeon.cqrs.core.commands;
using dungeon.cqrs.core.exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace dungeon.cqrs.implementation.commands
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        private static MethodInfo dispatchCmdMethodInfo = null;
        private static MethodInfo dispatchCmdWithReturnMethodInfo = null;
        

        public CommandDispatcher(ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.logger = loggerFactory.CreateLogger<CommandDispatcher>();

            if (dispatchCmdMethodInfo == null || dispatchCmdWithReturnMethodInfo == null)
            {
                var dipatchMethods = this.GetType().GetMethods().Where(m => m.Name == "Dispatch").ToList();
                dispatchCmdMethodInfo = dipatchMethods.First(m => m.GetGenericArguments().Count() == 1);
                dispatchCmdWithReturnMethodInfo = dipatchMethods.First(m => m.GetGenericArguments().Count() == 2);
            }
        }

        public async Task DispatchAbstract(ICommand command, int concurencyRetry = 0)
        {
            var method = dispatchCmdMethodInfo.MakeGenericMethod(command.GetType());
            await (Task)method.Invoke(this, new object[] { command, concurencyRetry });
        }

        public async Task<TR> DispatchAbstract<TR>(ICommand command, int concurencyRetry = 0)
        {
            var method = dispatchCmdWithReturnMethodInfo.MakeGenericMethod(typeof(TR), command.GetType());
            return await (Task<TR>)method.Invoke(this, new object[] { command, concurencyRetry });
        }

        public async Task Dispatch<TC>(TC command, int concurencyRetry = 0) where TC : ICommand
        {
            var handler = serviceProvider.GetService<ICommandHandler<TC>>();

            while (true)
            {
                try
                {
                    await handler.Handle(command);
                    return;
                }
                catch (DungeonConcurrencyException e)
                {
                    logger.LogWarning($"Concurrency promebs with command: {command.GetType().FullName}. Attempts left: {concurencyRetry}");
                    logger.LogWarning(e.ToString());

                    if (concurencyRetry-- <= 0)
                        throw e;
                }
            }
        }

        public async Task<TR> Dispatch<TR, TC>(TC command, int concurencyRetry = 0) where TC : ICommand
        {
            var handler = serviceProvider.GetService<ICommandHandler<TR, TC>>();

            while (true)
            {
                try
                {
                    return await handler.Handle(command);
                }
                catch (DungeonConcurrencyException e)
                {
                    logger.LogWarning($"Concurrency promebs with command: {command.GetType().FullName}. Attempts left: {concurencyRetry}");
                    logger.LogWarning(e.ToString());
                    if (concurencyRetry-- <= 0)
                        throw e;
                }
            }
        }
    }
}