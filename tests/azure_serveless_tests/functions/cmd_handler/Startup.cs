using common.utils;
using dungeon.cqrs.core;
using dungeon.cqrs.implementation;
using dungeon.cqrs.azure;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using dungeon.cqrs.core.event_sourcing.models;

[assembly: FunctionsStartup(typeof(cmd_handler.Startup))]
namespace cmd_handler
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var azureTablesConfig = new AzureTablesEventStoreConfig 
            {
                ConnectionString = "AzureWebJobsStorage",
                CommandsTableName = "dungeoncqrscommands",
                EventsTableName = "dungeoncqrsevents"
            };

            var azureSnapshotsConfig = new AzureTablesSnapshotsConfig 
            {
                SnapshotsTableName = "dungeoncqrssnapshots"
            };

            builder.Services
                .AddCqrs<AzureLoggerWrapper>()
                .AddAzureTablesEventSourcing(azureTablesConfig)
                .WithAzureTablesSnapshots(azureSnapshotsConfig, 
                    cfg=> cfg.SetDefaultSnapshotConfig(SnapshotConfig.VersionDiff(5)))
                .AndCreateAzureTables()
                .Build();
        }
    }
}