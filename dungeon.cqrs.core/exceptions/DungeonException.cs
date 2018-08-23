using System;

namespace dungeon.cqrs.core.exceptions {
    public class DungeonException : Exception {
        public DungeonException () { }

        public DungeonException (string message) : base (message) { }

        public DungeonException (string message, Exception exception) : base (message, exception) { }
    }
}