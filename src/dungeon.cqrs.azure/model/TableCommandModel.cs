using Microsoft.WindowsAzure.Storage.Table;

namespace dungeon.cqrs.azure.model
{
    public class TableCommandModel : TableEntity
    {
        public string CommandTypeHint {get; set;}
        public string CommandJsonData {get; set;}
    }
}
