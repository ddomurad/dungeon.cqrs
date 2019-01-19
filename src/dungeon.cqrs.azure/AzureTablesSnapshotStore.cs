using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;

using dungeon.cqrs.core.event_sourcing;
using dungeon.cqrs.core.event_sourcing.models;
using dungeon.cqrs.azure.model;

namespace dungeon.cqrs.azure
{
    public class AzureTablesSnapshotStore : ISnapshotStore
    {
        private readonly IAzureModelSerializer modelSerializer;

        private readonly CloudTable snapshotsTable;

        public AzureTablesSnapshotStore(AzureTablesSnapshotsConfig config)
        {
            var cloudStorageaccount = CloudStorageAccount.Parse(config.ConnectionString);
            var tableClient = cloudStorageaccount.CreateCloudTableClient();
            
            snapshotsTable = tableClient.GetTableReference(config.SnapshotsTableName);

            this.modelSerializer = config.ModelSerializer;
        }

        public async Task CreateTablesIfNotCreated()
        {
            await snapshotsTable.CreateIfNotExistsAsync();
        }

        public async Task<Memento> Read(string aggregateId)
        {
            var tableQuery = new TableQuery<TableMementoModel>().Where(
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, aggregateId)
            );

            var result = await snapshotsTable.ExecuteQuerySegmentedAsync(tableQuery, null);
            return MapToDungeonModel(result.FirstOrDefault());
        }

        public async Task<ulong> ReadVersion(string aggregateId)
        {
            var memento = await Read(aggregateId);
            return memento?.Metadata.Version ?? 0;
        }

        public async Task Write(Memento memento)
        {
            var tableObj = MapToAzureModel(memento);
            var tableInsertOperation = TableOperation.Insert(tableObj);
            
            await snapshotsTable.ExecuteAsync(tableInsertOperation);
        }

        private TableMementoModel MapToAzureModel(Memento obj)
        {
            return new TableMementoModel {
                PartitionKey = obj.Metadata.AggregateId,
                RowKey  = obj.Metadata.AggregateId,
                MementoTypeHint = modelSerializer.GetTypeHint(obj),
                MementoJsonData = modelSerializer.Serialize(obj)
            };
        }

        private Memento MapToDungeonModel(TableMementoModel obj)
        {
            if(obj == null)
                return null;

            return modelSerializer.Deserialize(obj.MementoTypeHint, obj.MementoJsonData) as Memento;
        }
    }
}
