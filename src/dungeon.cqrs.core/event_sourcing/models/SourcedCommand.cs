using System;
using dungeon.cqrs.core.commands;

namespace dungeon.cqrs.core.event_sourcing.models
{
    public class SourcedCommand : ICommand
    {
        public Guid CommandId { get; set; } = Guid.NewGuid();
    }
}