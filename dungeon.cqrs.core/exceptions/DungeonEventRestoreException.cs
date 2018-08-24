namespace dungeon.cqrs.core.exceptions
{
    public class DungeonEventRestoreException : DungeonException
    {
        public DungeonEventRestoreException() { }

        public DungeonEventRestoreException(string message) : base(message) { }
    }
}