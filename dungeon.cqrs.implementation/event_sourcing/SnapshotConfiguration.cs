using System;
using System.Collections.Concurrent;
using dungeon.cqrs.core.event_sourcing;
using dungeon.cqrs.core.event_sourcing.models;
using dungeon.cqrs.core.exceptions;


namespace dungeon.cqrs.event_sourcing {
    public class SnapshotConfiguration : ISnapshotConfiguration {
        private SnapshotConfig defaultSnapshotConfig = null;
        private readonly ConcurrentDictionary<Type, SnapshotConfig> entitySnapshotsConfigs = new ConcurrentDictionary<Type, SnapshotConfig> ();

        public void SetDefaultSnapshotConfig (SnapshotConfig config) {
            defaultSnapshotConfig = config;
        }

        public void AddSnapshotConfig<T> (SnapshotConfig config) {
            var entityType = typeof (T);
            if (!entitySnapshotsConfigs.TryAdd (entityType, config))
                throw new DungeonException ($"Snapshot configration for given entity type already exists: {entityType.FullName}");
        }

        public SnapshotConfig GetConfiguration (IAggregateRoot aggregate) {
            if (entitySnapshotsConfigs.TryGetValue (aggregate.GetType (), out var config))
                return config;

            return defaultSnapshotConfig;
        }
    }
}