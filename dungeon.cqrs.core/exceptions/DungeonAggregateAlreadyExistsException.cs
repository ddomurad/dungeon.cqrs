namespace dungeon.cqrs.core.exceptions {
    public class DungeonAggregateAlreadyExistsException : DungeonException {
        public DungeonAggregateAlreadyExistsException () { }

        public DungeonAggregateAlreadyExistsException (string message) : base (message) { }
    }
}