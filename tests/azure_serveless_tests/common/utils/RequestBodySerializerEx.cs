using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace common.utils
{
    public static class RequestBodySerializerEx
    {
        public static async Task<T> GetBodyAsync<T>(this HttpRequest request)
        {
            string requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            return JsonConvert.DeserializeObject<T>(requestBody);
        }

        public static T GetBody<T>(this HttpRequest request)
        {
            string requestBody = new StreamReader(request.Body).ReadToEnd();
            return JsonConvert.DeserializeObject<T>(requestBody);
        }
    }
}
