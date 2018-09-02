using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using dungeon.cqrs.core.events;
using dungeon.cqrs.core.exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace dungeon.cqrs.implementation.priv {
    public class EventHandlerTypesProvider {
        private readonly Dictionary<Type, List<Type>> eventsHandlersTypes = new Dictionary<Type, List<Type>> ();

        public void RegisterAssembly (Assembly assembly) {
            assembly.GetTypes ().ToList ().ForEach (RegisterType);
        }

        public void RegisterType (Type handlerType) {
            handlerType.GetInterfaces ().ToList ().ForEach (t => RegisterInterface (t, handlerType));
        }

        public void Build (IServiceCollection collection) {
            var method = typeof (EventHandlerTypesProvider).GetMethod ("PairEventToHandler");

            foreach (var kv in eventsHandlersTypes) {
                var tm = method.MakeGenericMethod (kv.Key);
                foreach (var ht in kv.Value)
                    tm.Invoke (this, new object[] { collection, ht });
            }
        }

        public void PairEventToHandler<Tevent> (IServiceCollection collection, Type t) where Tevent : IEvent {
            collection.AddTransient (typeof (IEventHandler<Tevent>), t);
        }

        private void RegisterInterface (Type interfaceType, Type handlerType) {
            if (!interfaceType.IsInterface)
                throw new DungeonException ($"RegisterInterface: {interfaceType.Name} is not an interface.");

            if (interfaceType == typeof (IEventHandler))
                return;

            if (interfaceType.GetInterface (typeof (IEventHandler).FullName) == null)
                return;

            var cmdType = interfaceType.GetGenericArguments ().Single ();

            if (!eventsHandlersTypes.ContainsKey (cmdType))
                eventsHandlersTypes.Add (cmdType, new List<Type> ());

            var eventHandlersTypesList = eventsHandlersTypes[cmdType];
            eventHandlersTypesList.Add (handlerType);
        }
    }
}