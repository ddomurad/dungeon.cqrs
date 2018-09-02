using System;
using dungeon.cqrs.core.events;

namespace dungeon.cqrs.core.wrappers
{
    public class BaseLoggerWrapper : ILoggerWrapper
    {
        public virtual void Error(string message) { }

        public virtual void Warning(string message) { }

        public virtual void Info(string message) { }

        public virtual void BeforeEventInvoke(IEvent e, IEventHandler handler) { }

        public virtual void AfterEventInvoke(IEvent e, IEventHandler handler) { }

        public virtual void OnEventInvokeError(IEvent e, IEventHandler handler, Exception ex) { }
    }
}