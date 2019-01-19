using System;
using dungeon.cqrs.event_sourcing;
using dungeon.cqrs.implementation;

namespace dungeon.cqrs.azure
{
    public class DungeonServiceCollectionEx_step3_withAzureCreateTables : DungeonServiceCollectionEx_step3
    {
        private readonly AzureTablesEventStoreConfig storeConfig;

        public DungeonServiceCollectionEx_step3_withAzureCreateTables(DungeonServiceCollectionEx_step self, AzureTablesEventStoreConfig storeConfig) : base(self)
        {
            this.storeConfig = storeConfig;
        }

        public DungeonServiceCollectionEx_step3 AndCreateAzureTables()
        {
            new AzureTablesEventStore(storeConfig).CreateTablesIfNotCreated().Wait();
            return this;
        }
    }

    public class DungeonServiceCollectionEx_step4_withAzureCreateTables : DungeonServiceCollectionEx_step4
    {
        private readonly AzureTablesSnapshotsConfig storeConfig;

        public DungeonServiceCollectionEx_step4_withAzureCreateTables(DungeonServiceCollectionEx_step self, AzureTablesSnapshotsConfig storeConfig) : base(self)
        {
            this.storeConfig = storeConfig;
        }

        public DungeonServiceCollectionEx_step4 AndCreateAzureTables()
        {
            new AzureTablesSnapshotStore(storeConfig).CreateTablesIfNotCreated().Wait();
            return this;
        }
    }

    public static class DungeonAzureTablesEventStoreEx
    {
        public static DungeonServiceCollectionEx_step3_withAzureCreateTables AddAzureTablesEventSourcing(this DungeonServiceCollectionEx_step2 step, AzureTablesEventStoreConfig storeConfig)
        {
            step.AddEventSourcing(() => new AzureTablesEventStore(storeConfig));
            return new DungeonServiceCollectionEx_step3_withAzureCreateTables(step, storeConfig);
        }

        public static DungeonServiceCollectionEx_step4_withAzureCreateTables WithAzureTablesSnapshots(this DungeonServiceCollectionEx_step3 step, AzureTablesSnapshotsConfig storeConfig, Action<SnapshotConfiguration> cfg)
        {
            step.WithSnapshots(() => new AzureTablesSnapshotStore(storeConfig), cfg);
            return new DungeonServiceCollectionEx_step4_withAzureCreateTables(step, storeConfig);
        }
    }
}
