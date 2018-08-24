namespace dungeon.cqrs.core.exceptions
{
    public class DungeonConcurrencyException : DungeonException
    {
        public DungeonConcurrencyException() { }

        public DungeonConcurrencyException(string message) : base(message) { }
    }
}