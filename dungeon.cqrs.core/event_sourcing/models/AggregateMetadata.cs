using System;

namespace dungeon.cqrs.core.event_sourcing.models
{
    public class AggregateMetadata : ICloneable
    {
        public string AggregateId { get; set; }
        public ulong Version { get; set; }
        public bool Exists { get; set; }

        public object Clone()
        {
            return new AggregateMetadata
            {
                Version = Version,
                Exists = Exists,
                AggregateId = AggregateId
            };
        }
    }
}