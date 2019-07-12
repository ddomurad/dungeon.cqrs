using dungeon.cqrs.core.event_sourcing.models;
using Newtonsoft.Json;

namespace common.utils
{
    public static class ObjectSerializerEx
    {
        public static T DeserializeObject<T>(this string str) where T: class
        {
            if(string.IsNullOrEmpty(str))
                return (T)null;
            
            return (T)JsonConvert.DeserializeObject<T>(str);
        }

        public static string SerializeObject<T>(this T obj) where T: class
        {
            if(obj == null)
                return null;

            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings{TypeNameHandling = TypeNameHandling.All});
        }
    }
}
