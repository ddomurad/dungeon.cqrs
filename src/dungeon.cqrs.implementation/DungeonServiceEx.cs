using System;
using System.Reflection;
using dungeon.cqrs.core.commands;
using dungeon.cqrs.core.event_sourcing;
using dungeon.cqrs.core.events;
using dungeon.cqrs.core.wrappers;
using dungeon.cqrs.event_sourcing;
using dungeon.cqrs.implementation.commands;
using dungeon.cqrs.implementation.events;
using dungeon.cqrs.implementation.priv;
using Microsoft.Extensions.DependencyInjection;

namespace dungeon.cqrs.implementation
{
    public static partial class DungeonServiceCollectionEx
    {
        public static DungeonServiceCollectionEx_step2 AddCqrs<TLogget>(this IServiceCollection serviceCollection) where TLogget : class, ILoggerWrapper
        {

            serviceCollection.AddSingleton<ILoggerWrapper, TLogget>();

            serviceCollection.AddScoped<ICommandDispatcher, CommandDispatcher>();
            serviceCollection.AddScoped<IEventSender, EventSender>();

            return new DungeonServiceCollectionEx_step2(new DungeonServiceCollectionEx_step(serviceCollection));
        }
    }

    public class DungeonServiceCollectionEx_step
    {
        protected IServiceCollection serviceCollection;

        public DungeonServiceCollectionEx_step(DungeonServiceCollectionEx_step self)
        {
            serviceCollection = self.serviceCollection;
        }

        public DungeonServiceCollectionEx_step(IServiceCollection serviceCollection)
        {
            this.serviceCollection = serviceCollection;
        }
    }

    public class DungeonServiceCollectionEx_step2 : DungeonServiceCollectionEx_step
    {
        public DungeonServiceCollectionEx_step2(DungeonServiceCollectionEx_step self) : base(self) { }

        public DungeonServiceCollectionEx_step3 AddEventSourcing<TEvetnStore>(Func<TEvetnStore> factoryMethod) where TEvetnStore : IEventStore
        {
            serviceCollection.AddScoped<IEventStore>(c => factoryMethod());
            serviceCollection.AddTransient<IEventSource, EventSource>();
            return new DungeonServiceCollectionEx_step3(this);
        }

        public DungeonServiceCollectionEx_step4 SkipEventSourcing()
        {
            return new DungeonServiceCollectionEx_step4(this);
        }
    }

    public class DungeonServiceCollectionEx_step3 : DungeonServiceCollectionEx_step
    {
        public DungeonServiceCollectionEx_step3(DungeonServiceCollectionEx_step self) : base(self) { }

        public DungeonServiceCollectionEx_step4 WithSnapshots(Func<ISnapshotStore> factoryMethod, Action<SnapshotConfiguration> cfg)
        {
            var config = new SnapshotConfiguration();
            cfg(config);

            serviceCollection.AddSingleton(typeof(ISnapshotConfiguration), config);
            serviceCollection.AddScoped<ISnapshotManager>(c => new SnapshotManager(factoryMethod(), c.GetService<ISnapshotConfiguration>()));

            return new DungeonServiceCollectionEx_step4(this);
        }

        public DungeonServiceCollectionEx_step4 WithoutSnapshots()
        {
            serviceCollection.AddScoped<ISnapshotManager, NullSnapshotManager>();
            return new DungeonServiceCollectionEx_step4(this);
        }
    }

    public class DungeonServiceCollectionEx_step4 : DungeonServiceCollectionEx_step
    {

        private EventHandlerTypesProvider eventHandlerTypesProvider;
        private CommandHandlerTypesProvider commandHandlerTypesProvider;

        public DungeonServiceCollectionEx_step4(DungeonServiceCollectionEx_step self) : base(self)
        {
            eventHandlerTypesProvider = new EventHandlerTypesProvider();
            commandHandlerTypesProvider = new CommandHandlerTypesProvider();
        }

        public DungeonServiceCollectionEx_step4 RegisterEventHandler(Type type)
        {
            eventHandlerTypesProvider.RegisterType(type);
            return this;
        }

        public DungeonServiceCollectionEx_step4 RegisterEventHandlers(Assembly assembly)
        {
            eventHandlerTypesProvider.RegisterAssembly(assembly);
            return this;
        }

        public DungeonServiceCollectionEx_step4 RegisterEventHandler<THandler>() where THandler : IEventHandler
        {
            RegisterEventHandler(typeof(THandler));
            return this;
        }

        public DungeonServiceCollectionEx_step4 RegisterCommandHandler(Type type)
        {
            commandHandlerTypesProvider.RegisterType(type);
            return this;
        }

        public DungeonServiceCollectionEx_step4 RegisterCommandHandlers(Assembly assembly)
        {
            commandHandlerTypesProvider.RegisterAssembly(assembly);
            return this;
        }

        public DungeonServiceCollectionEx_step4 RegisterCommandHandler<THandler>() where THandler : ICommandHandler
        {
            RegisterCommandHandler(typeof(THandler));
            return this;
        }

        public IServiceCollection Build()
        {
            commandHandlerTypesProvider.Build(serviceCollection);
            eventHandlerTypesProvider.Build(serviceCollection);
            return serviceCollection;
        }
    }
}