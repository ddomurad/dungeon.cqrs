namespace dungeon.cqrs.core.exceptions {
    public class DungeonInvalidConstructorException : DungeonException {
        public DungeonInvalidConstructorException () { }

        public DungeonInvalidConstructorException (string message) : base (message) { }
    }
}