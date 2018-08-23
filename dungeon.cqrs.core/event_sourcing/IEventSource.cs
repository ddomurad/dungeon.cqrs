using System.Threading.Tasks;
using dungeon.cqrs.core.event_sourcing.models;

namespace dungeon.cqrs.core.event_sourcing {
    public interface IEventSource {
        Task<T> CreateNew<T> (string aggregateId) where T : class, IAggregateRoot;
        Task<T> Pull<T> (string aggregateId) where T : class, IAggregateRoot;
        Task PushChanges (SourcedCommand command);
    }
}