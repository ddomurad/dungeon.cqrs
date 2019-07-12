using System;
using common.globals;
using common.utils;
using dungeon.cqrs.core.event_sourcing.models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace cmd_handler
{
    public static class dispatch_message
    {
        [FunctionName("dispatch_message")]
        public static void Run([QueueTrigger(QueuesNames.CommandsQueue, Connection = "AzureWebJobsStorage")]string stringCommand, ILogger log)
        {
            var command = stringCommand.DeserializeObject<SourcedCommand>();
            System.Console.WriteLine("Elo !");
        }
    }
}
