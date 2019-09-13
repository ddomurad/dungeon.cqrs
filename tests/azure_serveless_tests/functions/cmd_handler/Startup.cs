using dungeon.cqrs.implementation;
using dungeon.cqrs.azure;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using dungeon.cqrs.core.event_sourcing.models;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(cmd_handler.Startup))]
namespace cmd_handler
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var azureTablesConfig = new AzureTablesEventStoreConfig(
                "DefaultEndpointsProtocol=https;AccountName=dungeoncqrsstore;AccountKey=0I4EoXrh7HJQAMrOli+hIKTYFtuyOfUlorFsSASDkK+ESCFqVqsZOhtnW54tZTQa1zl4ZwA+JE8XDayHEk7zpA==;EndpointSuffix=core.windows.net",
                "dungeoncqrscommands",
                "dungeoncqrsevents"
            );

            var azureSnapshotsConfig = new AzureTablesSnapshotsConfig(
                "DefaultEndpointsProtocol=https;AccountName=dungeoncqrsstore;AccountKey=0I4EoXrh7HJQAMrOli+hIKTYFtuyOfUlorFsSASDkK+ESCFqVqsZOhtnW54tZTQa1zl4ZwA+JE8XDayHEk7zpA==;EndpointSuffix=core.windows.net",
                "dungeoncqrssnapshots"
            );

            builder.Services
                .AddLogging()
                .AddCqrs()
                .AddAzureTablesEventSourcing(azureTablesConfig)
                    .AndCreateAzureTables()
                .WithAzureTablesSnapshots(azureSnapshotsConfig, 
                    cfg=> cfg.SetDefaultSnapshotConfig(SnapshotConfig.VersionDiff(5)))
                    .AndCreateAzureTables()
                .RegisterCommandHandler<TestHandler>()
                .Build();
        }
    }
}