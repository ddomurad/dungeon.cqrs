using System;
using System.Linq;
using System.Threading.Tasks;
using dungeon.cqrs.core.commands;
using dungeon.cqrs.core.diagnostics;
using dungeon.cqrs.core.events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace dungeon.cqrs.implementation.events
{
    public class EventSender : IEventSender
    {
        private readonly IDiagnosticsFormater diagnosticsFormater;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;

        public EventSender(IDiagnosticsFormater diagnosticsFormater, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.diagnosticsFormater = diagnosticsFormater;
            this.logger = loggerFactory.CreateLogger<EventSender>();
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
                    this.logger.LogInformation(diagnosticsFormater.BeforeEventInvoke(e, handler));

                await handler.Handle(e, c);

                if (DungeonGlobalConfiguration.EventTrace)
                    this.logger.LogInformation(diagnosticsFormater.AfterEventInvoke(e, handler));
            }
            catch (Exception ex)
            {
                if (DungeonGlobalConfiguration.EventTrace)
                    this.logger.LogError(diagnosticsFormater.OnEventInvokeError(e, handler, ex));
                else 
                    this.logger.LogError(ex.Message, ex);
            }
        }
    }
}