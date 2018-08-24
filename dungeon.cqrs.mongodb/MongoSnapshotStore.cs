using System.Threading.Tasks;
using dungeon.cqrs.core.event_sourcing;
using dungeon.cqrs.core.event_sourcing.models;
using dungeon.cqrs.mongodb.model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace dungeon.cqrs.mongodb
{
    public class MongoSnapshotStore : ISnapshotStore
    {
        private readonly IMongoDatabase mongoDatabase;

        public MongoSnapshotStore(string connectionString, string dbName)
        {
            var client = new MongoClient(connectionString);
            mongoDatabase = client.GetDatabase(dbName);
        }

        public async Task<Memento> Read(string aggregateId)
        {
            var collection = mongoDatabase.GetCollection<SnapshotWrapper>("snapshots");
            return (await (await collection.FindAsync(f => f.AggId == aggregateId)).FirstOrDefaultAsync())?.Memento;
        }

        public async Task<ulong> ReadVersion(string aggregateId)
        {
            //TODO FIX
            var memento = await Read(aggregateId);
            return memento?.Metadata.Version ?? 0;
        }

        public async Task Write(Memento memento)
        {
            var collection = mongoDatabase.GetCollection<SnapshotWrapper>("snapshots");
            var em = (await (await collection.FindAsync(f => f.AggId == memento.Metadata.AggregateId))
                .FirstOrDefaultAsync());

            if (em == null)
                em = new SnapshotWrapper
                {
                    Id = ObjectId.GenerateNewId(),
                    AggId = memento.Metadata.AggregateId,
                    Memento = memento
                };
            else
                em.Memento = memento;

            await collection.ReplaceOneAsync(Builders<SnapshotWrapper>.Filter.Eq(u => u.Id, em.Id), em,
                new UpdateOptions { IsUpsert = true });
        }
    }
}