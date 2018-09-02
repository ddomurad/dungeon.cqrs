using dungeon.cqrs.core.wrappers;

namespace dungeon.cqrs.test
{
    public class ConsoleLogger : BaseLoggerWrapper
    {
        public override void Error(string message) { System.Console.WriteLine("ERROR: " + message); }

        public override void Warning(string message) { System.Console.WriteLine("WARN: " + message); }
    }
}