namespace dungeon.cqrs.core.event_sourcing.models {
    public class SnapshotConfig {
        public ulong TakeSnapshotWhenVersionDiferrenceIsBiggerThanOrEqual { get; set; }

        private SnapshotConfig () { }

        public static SnapshotConfig VersionDiff (ulong diff) {
            return new SnapshotConfig { TakeSnapshotWhenVersionDiferrenceIsBiggerThanOrEqual = diff };
        }

        public static SnapshotConfig Disabled () {
            return null;
        }
    }
}