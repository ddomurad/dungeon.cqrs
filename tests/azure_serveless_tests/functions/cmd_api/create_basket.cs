using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using common.utils;
using common.globals;
using common.command;

namespace cmd_api
{
    public class create_basket
    {
        [FunctionName("create_basket")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Queue(QueuesNames.CommandsQueue, Connection = "AzureWebJobsStorage")] out string queueMessage,
            ILogger log)
        {
            if(!req.Query.ContainsKey("name"))
                throw new Exception("Missing basket name");
            
            queueMessage = new AddBasketCmd 
            {
                BasketId = Guid.NewGuid().ToString(),
                BasketName = req.Query["name"]
            }.SerializeObject();
            
            return (ActionResult)new OkObjectResult($"New command pushed to queue");
        }
    }
}
