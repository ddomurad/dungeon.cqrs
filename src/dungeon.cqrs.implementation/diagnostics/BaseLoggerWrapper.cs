using System;
using dungeon.cqrs.core.diagnostics;
using dungeon.cqrs.core.events;
using Microsoft.Extensions.Logging;

namespace dungeon.cqrs.implementation.diagnostics
{
    public class DiagnosticsFormater : IDiagnosticsFormater
    {
        public virtual string BeforeEventInvoke(IEvent e, IEventHandler handler) 
        {
            return "BeforeEventInvoke";
        }

        public virtual string AfterEventInvoke(IEvent e, IEventHandler handler) 
        {
            return "AfterEventInvoke";
        }

        public virtual string OnEventInvokeError(IEvent e, IEventHandler handler, Exception ex) 
        {
            return "OnEventInvokeError";
        }
    }
}