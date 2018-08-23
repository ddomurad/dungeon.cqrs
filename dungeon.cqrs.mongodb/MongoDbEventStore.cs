using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dungeon.cqrs.core.event_sourcing;
using dungeon.cqrs.core.event_sourcing.models;
using dungeon.cqrs.core.exceptions;
using dungeon.cqrs.mongodb.model;
using MongoDB.Driver;

namespace dungeon.cqrs.mongodb
{
    public class MongoDbEventStore : IEventStore
    {
        private readonly IMongoDatabase mongoDatabase;

        public MongoDbEventStore(string connectionString, string dbName)
        {
            var client = new MongoClient(connectionString);
            mongoDatabase = client.GetDatabase(dbName);
        }

        public async Task SaveEvents(IEnumerable<SourcedEvent> events)
        {
            IMongoCollection<SourcedEvent> collection = GetEventsCollection();

            try
            {
                await collection.InsertManyAsync(events);
            }
            catch (MongoBulkWriteException e)
            {
                if (e.WriteErrors.SingleOrDefault()?.Category == ServerErrorCategory.DuplicateKey)
                    throw new DungeonConcurrencyException();
                else
                    throw;
            }
        }

        public async Task<IEnumerable<SourcedEvent>> ReadEvents(string aggregateId)
        {
            var collection = GetEventsCollection();
            var filter = Builders<SourcedEvent>.Filter.Where(e => e.Metadata.AggregateId == aggregateId);
            var sort = Builders<SourcedEvent>.Sort.Ascending(e => e.Metadata.Version);

            return (await collection.Find(filter).Sort(sort).ToListAsync()).AsEnumerable();
        }

        public async Task<IEnumerable<SourcedEvent>> ReadEvents(string aggregateId, ulong fromVersion)
        {
            var collection = GetEventsCollection();
            var filter = Builders<SourcedEvent>.Filter.Where(e => e.Metadata.AggregateId == aggregateId && e.Metadata.Version > fromVersion);
            var sort = Builders<SourcedEvent>.Sort.Ascending(e => e.Metadata.Version);

            return (await collection.Find(filter).Sort(sort).ToListAsync()).AsEnumerable();
        }

        public async Task<IEnumerable<SourcedEvent>> ReadEvents(string aggregateId, Guid cmdId)
        {
            var collection = GetEventsCollection();
            var filter = Builders<SourcedEvent>.Filter.Where(e => e.Metadata.AggregateId == aggregateId && e.Metadata.CommandId == cmdId);
            var sort = Builders<SourcedEvent>.Sort.Ascending(e => e.Metadata.Version);

            return (await collection.Find(filter).Sort(sort).ToListAsync()).AsEnumerable();
        }

        public async Task<IEnumerable<SourcedEvent>> ReadAllEvents(DateTime? olderThan)
        {
            var collection = GetEventsCollection();

            var filter = olderThan == null ?
                Builders<SourcedEvent>.Filter.Where(e => true) :
                Builders<SourcedEvent>.Filter.Where(e => e.Metadata.Date <= olderThan.Value);

            var sort = Builders<SourcedEvent>.Sort.Ascending(e => e.Metadata.Version);

            return (await collection.Find(filter).Sort(sort).ToListAsync()).AsEnumerable();
        }

        public async Task<IEnumerable<SourcedEvent>> ReadAllEvents(string aggregateId, DateTime? olderThan)
        {
            var collection = GetEventsCollection();

            var filter = olderThan == null ?
                Builders<SourcedEvent>.Filter.Where(e => e.Metadata.AggregateId == aggregateId) :
                Builders<SourcedEvent>.Filter.Where(e => e.Metadata.AggregateId == aggregateId && e.Metadata.Date <= olderThan.Value);

            var sort = Builders<SourcedEvent>.Sort.Ascending(e => e.Metadata.Version);

            return (await collection.Find(filter).Sort(sort).ToListAsync())
                .Select(e => e).AsEnumerable();
        }

        public async Task SaveCommand(SourcedCommand command)
        {
            var collection = mongoDatabase.GetCollection<SourcedCommand>("Commands");
            await collection.InsertOneAsync(command);
        }

        private IMongoCollection<SourcedEvent> GetEventsCollection()
        {
            return mongoDatabase.GetCollection<SourcedEvent>("Events");
        }
    }
}