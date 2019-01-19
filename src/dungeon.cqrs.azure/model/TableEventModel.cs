using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace dungeon.cqrs.azure.model
{
    public class TableEventModel : TableEntity
    {
        public Guid CommandId {get; set;}
        
        public long EventVersion {get; set;}
        public DateTime EventDate {get; set;}

        public string EventTypeHint {get; set;}
        public string EventJsonData {get; set;}
    }
}
