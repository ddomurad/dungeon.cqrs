namespace dungeon.cqrs.azure
{
    public interface IAzureModelSerializer
    {
        string GetTypeHint(object obj);
        string Serialize(object obj);

        object Deserialize(string typeHint, string data);
    }
}
