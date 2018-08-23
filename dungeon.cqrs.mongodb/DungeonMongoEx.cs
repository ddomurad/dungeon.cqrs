using System;
using dungeon.cqrs.event_sourcing;
using dungeon.cqrs.implementation;

namespace dungeon.cqrs.mongodb
{
    public static class DungeonMongoEx {
        public static DungeonServiceCollectionEx_step3 AddMongoEventSourcing(this DungeonServiceCollectionEx_step2 step, string connectionString, string dbName)  {
            step.AddEventSourcing(() => new MongoDbEventStore (connectionString, dbName));
            return new DungeonServiceCollectionEx_step3(step);
        }

        public static DungeonServiceCollectionEx_step4 WithMongoSnapshots (this DungeonServiceCollectionEx_step3 step,string connectionString, string dbName, Action<SnapshotConfiguration> cfg) {
            step.WithSnapshots(()=> new MongoSnapshotStore(connectionString, dbName), cfg);
            return new DungeonServiceCollectionEx_step4 (step);
        }
    }
}