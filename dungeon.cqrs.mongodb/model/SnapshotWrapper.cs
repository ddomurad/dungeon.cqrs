using dungeon.cqrs.core.event_sourcing.models;
using MongoDB.Bson;

namespace dungeon.cqrs.mongodb.model {
    public class SnapshotWrapper {
        public ObjectId Id { get; set; }
        public string AggId { get; set; }
        public Memento Memento { get; set; }
    }
}