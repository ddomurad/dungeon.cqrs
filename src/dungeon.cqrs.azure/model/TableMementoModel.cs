using Microsoft.WindowsAzure.Storage.Table;

namespace dungeon.cqrs.azure.model
{
    public class TableMementoModel : TableEntity
    {
        public string MementoTypeHint {get; set;}
        public string MementoJsonData {get; set;}
    }
}
