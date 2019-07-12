using System;
using dungeon.cqrs.core.events;
using dungeon.cqrs.core.wrappers;
using Microsoft.Extensions.Logging;

namespace common.utils 
{
    public class AzureLoggerWrapper : ILoggerWrapper
    {
        ILogger _log;
        public AzureLoggerWrapper(ILogger log)
        {
            _log = log;
        }

        public void AfterEventInvoke(IEvent e, IEventHandler handler)
        {}

        public void BeforeEventInvoke(IEvent e, IEventHandler handler)
        {}

        public void Error(string message)
        {
            _log.LogError(message);
        }

        public void Info(string message)
        {
            _log.LogInformation(message);
        }

        public void OnEventInvokeError(IEvent e, IEventHandler handler, Exception ex)
        {
            Error("OnEventInvokeError");
        }

        public void Warning(string message)
        {
            _log.LogWarning(message);
        }
    }
}