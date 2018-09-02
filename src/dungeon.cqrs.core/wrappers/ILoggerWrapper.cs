using System;
using dungeon.cqrs.core.events;

namespace dungeon.cqrs.core.wrappers
{
    public interface ILoggerWrapper
    {
        void Error(string message);
        void Warning(string message);
        void Info(string message);

        void BeforeEventInvoke(IEvent e, IEventHandler handler);
        void AfterEventInvoke(IEvent e, IEventHandler handler);
        void OnEventInvokeError(IEvent e, IEventHandler handler, Exception ex);
    }
}