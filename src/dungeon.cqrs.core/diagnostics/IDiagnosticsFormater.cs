using System;
using dungeon.cqrs.core.events;

namespace dungeon.cqrs.core.diagnostics
{
    public interface IDiagnosticsFormater
    {
        string BeforeEventInvoke(IEvent e, IEventHandler handler);
        string AfterEventInvoke(IEvent e, IEventHandler handler);
        string OnEventInvokeError(IEvent e, IEventHandler handler, Exception ex);
    }
}