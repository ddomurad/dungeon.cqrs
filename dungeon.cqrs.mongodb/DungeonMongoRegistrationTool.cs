using System;
using System.Reflection;
using dungeon.cqrs.core.event_sourcing.models;
using MongoDB.Bson.Serialization;

namespace dungeon.cqrs.mongodb {
    public static class DungeonMongoRegistrationTool {
        public static void RegisterInternals () {
            BsonClassMap.RegisterClassMap<SourcedCommand>(map => {
                map.MapIdField(c => c.CommandId);
                map.AutoMap();
            });

            BsonClassMap.RegisterClassMap<SourcedEvent>(map => {
                map.MapIdField(e => e.EventId);
                map.AutoMap();
            });

            RegisterAll (typeof (AggregateCreatedEvent).Assembly);
            BsonSerializer.RegisterSerializer (typeof (DateTime), new DungeonDateTimeSerializer ());

        }

        public static void RegisterAll (Assembly assembly) {
            var types = assembly.GetTypes ();
            foreach (var type in types) {
                if (type.IsSubclassOf (typeof (SourcedEvent)) ||
                    type.IsSubclassOf (typeof (SourcedCommand)) ||
                    type.IsSubclassOf (typeof (Memento))) {
                    var method = typeof (DungeonMongoRegistrationTool).GetMethod ("RegisterSingle");
                    var typedMethod = method.MakeGenericMethod (type);
                    typedMethod.Invoke (null, null);
                }
            }
        }

        public static void RegisterSingle<T> () {
            if (!BsonClassMap.IsClassMapRegistered (typeof (T)))
                BsonClassMap.RegisterClassMap<T> (map => map.AutoMap ());
        }
    }
}