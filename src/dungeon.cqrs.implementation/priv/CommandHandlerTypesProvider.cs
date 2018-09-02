using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using dungeon.cqrs.core.commands;
using dungeon.cqrs.core.exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace dungeon.cqrs.implementation.priv {
    public class CommandHandlerTypesProvider {

        private readonly Dictionary<Type, Type> handlerTypeMap = new Dictionary<Type, Type> ();
        private readonly Dictionary<Type, Type> handlerReturnTypeMap = new Dictionary<Type, Type> ();

        public void RegisterAssembly (Assembly assembly) {
            assembly.GetTypes ().ToList ().ForEach (RegisterType);
        }

        public void RegisterType (Type handlerType) {
            handlerType.GetInterfaces ().ToList ().ForEach (t => RegisterInterface (t, handlerType));
        }

        public void Build (IServiceCollection collection) {
            var method = typeof (CommandHandlerTypesProvider).GetMethod ("PairEventToHandler");
            var methodEx = typeof (CommandHandlerTypesProvider).GetMethod ("PairEventToHandlerEx");

            foreach (var kv in handlerTypeMap) {
                if (handlerReturnTypeMap.ContainsKey (kv.Key)) {
                    var tm = methodEx.MakeGenericMethod (handlerReturnTypeMap[kv.Key], kv.Key);
                    tm.Invoke (this, new object[] { collection, kv.Value });
                } else {
                    var tm = method.MakeGenericMethod (kv.Key);
                    tm.Invoke (this, new object[] { collection, kv.Value });
                }
            }
        }

        public void PairEventToHandler<Tcommand> (IServiceCollection collection, Type t) where Tcommand : ICommand {
            collection.AddTransient (typeof (ICommandHandler<Tcommand>), t);
        }

        public void PairEventToHandlerEx<Treturn, Tcommand> (IServiceCollection collection, Type t) where Tcommand : ICommand {
            collection.AddTransient (typeof (ICommandHandler<Treturn, Tcommand>), t);
        }

        private void RegisterInterface (Type interfaceType, Type handlerType) {

            if (!interfaceType.IsInterface)
                throw new DungeonException ($"RegisterInterface: {interfaceType.Name} is not an interface.");

            if (interfaceType == typeof (ICommandHandler))
                return;

            if (interfaceType.GetInterface (typeof (ICommandHandler).FullName) == null)
                return;

            var genericArgs = interfaceType.GetGenericArguments ().ToList ();
            var cmdType = genericArgs.Last ();

            if (handlerTypeMap.ContainsKey (cmdType))
                throw new DungeonException ($"RegisterInterface: Command handler already registered: '{cmdType.FullName}'");

            handlerTypeMap.Add (cmdType, handlerType);

            if (genericArgs.Count () > 1) {
                if (handlerReturnTypeMap.ContainsKey (cmdType))
                    throw new DungeonException ($"RegisterInterface: Command handler already registered: '{cmdType.FullName}'");
                handlerReturnTypeMap.Add (cmdType, genericArgs.First ());
            }
        }

    }
}