using Newtonsoft.Json;

namespace dungeon.cqrs.azure
{
    public class DefaultModelSerializer : IAzureModelSerializer
    {
        public string GetTypeHint(object obj)
        {
            return obj.GetType().FullName;
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings{TypeNameHandling = TypeNameHandling.All});
        }

        public object Deserialize(string typeHint, string data)
        {
            return JsonConvert.DeserializeObject(data, new JsonSerializerSettings{TypeNameHandling = TypeNameHandling.All});
        }

    }
}
