using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;

using dungeon.cqrs.core.event_sourcing;
using dungeon.cqrs.core.event_sourcing.models;
using dungeon.cqrs.core.exceptions;
using dungeon.cqrs.azure.model;

namespace dungeon.cqrs.azure
{

    public class AzureTablesEventStore : IEventStore
    {
        private readonly IAzureModelSerializer modelSerializer;

        private readonly CloudTable commandsTable;
        private readonly CloudTable eventsTable;


        public AzureTablesEventStore(AzureTablesEventStoreConfig config)
        {
            var cloudStorageaccount = CloudStorageAccount.Parse(config.ConnectionString);
            var tableClient = cloudStorageaccount.CreateCloudTableClient();
            
            commandsTable = tableClient.GetTableReference(config.CommandsTableName);
            eventsTable = tableClient.GetTableReference(config.EventsTableName);

            this.modelSerializer = config.ModelSerializer;
        }

        public async Task CreateTablesIfNotCreated()
        {
            await commandsTable.CreateIfNotExistsAsync();
            await eventsTable.CreateIfNotExistsAsync();
        }

        public async Task<IEnumerable<SourcedEvent>> ReadAllEvents(DateTime? replayTill)
        {
            TableQuery<TableEventModel> tableQuery = null;

            if(replayTill.HasValue)
                tableQuery = new TableQuery<TableEventModel>().Where(
                    TableQuery.GenerateFilterConditionForDate("EventDate", QueryComparisons.LessThanOrEqual, replayTill.Value));
            else
                tableQuery = new TableQuery<TableEventModel>();
            
            return await PullEvents(tableQuery);
        }

        public async Task<IEnumerable<SourcedEvent>> ReadAllEvents(string aggregateId, DateTime? replayTill)
        {
            TableQuery<TableEventModel> tableQuery = null;

            if(replayTill.HasValue)
                tableQuery = new TableQuery<TableEventModel>().Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, aggregateId),
                        TableOperators.And,
                        TableQuery.GenerateFilterConditionForDate("EventDate", QueryComparisons.LessThanOrEqual, replayTill.Value)
                    ));
            else
                tableQuery = new TableQuery<TableEventModel>().Where(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, aggregateId)
                );
            
            return await PullEvents(tableQuery);
        }

        public async Task<IEnumerable<SourcedEvent>> ReadEvents(string aggregateId)
        {
            var tableQuery = new TableQuery<TableEventModel>().Where(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, aggregateId));
            
            return await PullEvents(tableQuery);
        }

        public async Task<IEnumerable<SourcedEvent>> ReadEvents(string aggregateId, ulong fromVersion)
        {   
            var tableQuery = new TableQuery<TableEventModel>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, aggregateId),
                    TableOperators.And,
                    TableQuery.GenerateFilterConditionForLong("EventVersion", QueryComparisons.GreaterThanOrEqual, (long)fromVersion)
                ));
            
            return await PullEvents(tableQuery);
        }

        public async Task<IEnumerable<SourcedEvent>> ReadEvents(string aggregateId, Guid cmdId)
        {
            var tableQuery = new TableQuery<TableEventModel>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, aggregateId),
                    TableOperators.And,
                    TableQuery.GenerateFilterConditionForGuid("CommandId", QueryComparisons.GreaterThanOrEqual, cmdId)
                ));
            
            return await PullEvents(tableQuery);
        }

        public async Task SaveCommand(SourcedCommand command)
        {
            var tableCommand = MapToAzureModel(command);
            var tableInsertOperation = TableOperation.Insert(tableCommand);
            
            await commandsTable.ExecuteAsync(tableInsertOperation);
        }

        public async Task SaveEvents(IEnumerable<SourcedEvent> events)
        {
            var batchOperation = new TableBatchOperation();

            var tableEvents = events.Select(MapToAzureModel).ToList();
            tableEvents.ForEach(te => batchOperation.Insert(te));
            try
            {
                await eventsTable.ExecuteBatchAsync(batchOperation);
            }catch(Microsoft.WindowsAzure.Storage.StorageException e)
            {
                if(e.Message.StartsWith("0:The specified entity already exists."))
                    throw new DungeonConcurrencyException();

                throw;
            }
        }

        private async Task<List<SourcedEvent>> PullEvents(TableQuery<TableEventModel> query)
        {
            return  (await ExecuteEventsQuery(query)).Select(MapToDungeonModel).ToList();
        }

        private async Task<List<TableEventModel>> ExecuteEventsQuery(TableQuery<TableEventModel> query)
        {
            var outModel = new List<TableEventModel>();
            TableContinuationToken continuationToken = null;

            do
            {
                var segment = await eventsTable.ExecuteQuerySegmentedAsync<TableEventModel>(query, continuationToken);
                continuationToken = segment.ContinuationToken;

                outModel.AddRange(segment.ToList());
            } while(continuationToken != null);

            outModel.OrderBy(e => e.EventVersion);
            return outModel;
        }

        private TableCommandModel MapToAzureModel(SourcedCommand command)
        {
            return new TableCommandModel {
                RowKey = command.CommandId.ToString(),
                PartitionKey = command.CommandId.ToString(),
                CommandTypeHint = modelSerializer.GetTypeHint(command),
                CommandJsonData = modelSerializer.Serialize(command)
            };
        }

        private TableEventModel MapToAzureModel(SourcedEvent @event)
        {
            return new TableEventModel {
                RowKey = @event.EventId.ToString(),
                PartitionKey = @event.Metadata.AggregateId.ToString(),
                EventTypeHint = modelSerializer.GetTypeHint(@event),
                EventDate = @event.Metadata.Date,
                EventVersion = (long)@event.Metadata.Version,
                CommandId = @event.Metadata.CommandId,
                EventJsonData = modelSerializer.Serialize(@event)
            };
        }
        private SourcedEvent MapToDungeonModel(TableEventModel @event)
        {
            return modelSerializer.Deserialize(@event.EventTypeHint, @event.EventJsonData) as SourcedEvent;
        }
    }
}
