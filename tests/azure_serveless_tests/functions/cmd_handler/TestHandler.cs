using System.Threading.Tasks;
using common.command;
using dungeon.cqrs.core.commands;
using Microsoft.Extensions.Logging;

namespace cmd_handler
{
    public class TestHandler : ICommandHandler<AddBasketCmd>
    {
        ILogger _logger;
        public TestHandler(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TestHandler>();
        }

        public Task Handle(AddBasketCmd command)
        {
            _logger.LogInformation("add basked cmd recevied :)");
            return Task.CompletedTask;
        }
    }
}
