using System;
using System.Threading.Tasks;
using common.globals;
using common.utils;
using dungeon.cqrs.core.commands;
using dungeon.cqrs.core.event_sourcing.models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace cmd_handler
{
    public class dispatch_message
    {
        private ICommandDispatcher _cmdDispatcher;
        public dispatch_message(ICommandDispatcher cmdDispatcher)
        {
            _cmdDispatcher = cmdDispatcher;
        }

        [FunctionName("dispatch_message")]
        public async Task Run([QueueTrigger(QueuesNames.CommandsQueue, Connection = "AzureWebJobsStorage")]string stringCommand, ILogger log)
        {
            var command = stringCommand.DeserializeObject<SourcedCommand>();
            await _cmdDispatcher.DispatchAbstract(command);
        }
    }
}
