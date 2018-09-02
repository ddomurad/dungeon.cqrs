using System;
using System.Linq;
using System.Threading.Tasks;
using dungeon.cqrs.core.commands;
using dungeon.cqrs.core.events;
using dungeon.cqrs.core.wrappers;
using Microsoft.Extensions.DependencyInjection;

namespace dungeon.cqrs.implementation.events
{
    public class EventSender : IEventSender
    {
        private readonly ILoggerWrapper loggerWrapper;
        private readonly IServiceProvider serviceProvider;

        public EventSender(ILoggerWrapper loggerWrapper, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.loggerWrapper = loggerWrapper;
        }

        public async Task Send<TE>(TE e) where TE : IEvent
        {
            var handlers = serviceProvider.GetServices<IEventHandler<TE>>();

            foreach (var handler in handlers)
                await FireEvent<TE>(handler, e, null);
        }

        public async Task Send<TE, TC>(TE e, TC c)
            where TE : IEvent
            where TC : ICommand
        {
            var handlers = serviceProvider.GetServices<IEventHandler<TE>>();

            foreach (var handler in handlers)
                await FireEvent<TE>(handler, e, c);
        }

        private async Task FireEvent<TE>(IEventHandler<TE> handler, TE e, ICommand c) where TE : IEvent
        {
            try
            {
                if (DungeonGlobalConfiguration.EventTrace)
                    loggerWrapper.BeforeEventInvoke(e, handler);

                await handler.Handle(e, c);

                if (DungeonGlobalConfiguration.EventTrace)
                    loggerWrapper.AfterEventInvoke(e, handler);

            }
            catch (Exception ex)
            {
                if (DungeonGlobalConfiguration.EventTrace)
                    loggerWrapper.OnEventInvokeError(e, handler, ex);

                loggerWrapper.Error(ex.ToString());
            }
        }
    }
}