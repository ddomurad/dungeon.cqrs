using System;
using System.Linq;

namespace dungeon.cqrs.azure
{
    public class BaseAzureTablesStoreConfig
    {
        public string ConnectionString { get; set; }
        public IAzureModelSerializer ModelSerializer { get => modelSerializer; set => modelSerializer = ValidateSerializer(value); }

        private IAzureModelSerializer modelSerializer = new DefaultModelSerializer();

        public BaseAzureTablesStoreConfig()
        {}


        protected IAzureModelSerializer ValidateSerializer(IAzureModelSerializer obj)
        {
            if(obj == null)
                throw new ArgumentException("ModelSerializer can't be null.");

            return obj;
        }

        protected string ValidateTableName(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName) || !tableName.All(c => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9')))
                throw new ArgumentException($"Invalid table name: {tableName}.");

            return tableName;
        }
    }

    public class AzureTablesEventStoreConfig : BaseAzureTablesStoreConfig
    { 
        public string CommandsTableName { get => commandsTableName; set => commandsTableName = ValidateTableName(value); }
        public string EventsTableName { get => eventsTableName; set => eventsTableName = ValidateTableName(value); }

        private string commandsTableName = "dungeonCommands";
        private string eventsTableName = "dungeonEvents";


        public AzureTablesEventStoreConfig()
        {}

        public AzureTablesEventStoreConfig(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public AzureTablesEventStoreConfig(string connectionString, string commandsTableName, string eventsTableName)
        {
            this.ConnectionString = connectionString;
            this.CommandsTableName = commandsTableName;
            this.EventsTableName = eventsTableName;
        }

    }

    public class AzureTablesSnapshotsConfig : BaseAzureTablesStoreConfig
    {
        public string SnapshotsTableName { get => snapshotsTableName; set => snapshotsTableName = ValidateTableName(value); }
        private string snapshotsTableName = "dungeonSnapshots";
     

        public AzureTablesSnapshotsConfig()
        {}

        public AzureTablesSnapshotsConfig(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public AzureTablesSnapshotsConfig(string connectionString, string snapshotsTableName)
        {
            this.ConnectionString = connectionString;
            this.SnapshotsTableName = snapshotsTableName;
        }
    }
}
