using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dungeon.cqrs.core.event_sourcing.models;

namespace dungeon.cqrs.core.event_sourcing {
    public interface IEventStore {
        Task SaveEvents (IEnumerable<SourcedEvent> events);

        Task<IEnumerable<SourcedEvent>> ReadEvents (string aggregateId);
        Task<IEnumerable<SourcedEvent>> ReadEvents (string aggregateId, ulong fromVersion);
        Task<IEnumerable<SourcedEvent>> ReadEvents (string aggregateId, Guid cmdId);

        Task<IEnumerable<SourcedEvent>> ReadAllEvents (DateTime? replayTill);
        Task<IEnumerable<SourcedEvent>> ReadAllEvents (string aggregateId, DateTime? replayTill);

        Task SaveCommand (SourcedCommand command);
    }
}