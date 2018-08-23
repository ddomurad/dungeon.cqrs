using System;
using System.Collections.Generic;
using System.Linq;
using dungeon.cqrs.core.event_sourcing;
using dungeon.cqrs.core.event_sourcing.models;
using dungeon.cqrs.core.exceptions;

namespace dungeon.cqrs.implementation.event_sourcing {
    public abstract class AggregateRoot : IAggregateRoot {
        public AggregateMetadata Metadata { get; set; }

        private readonly List<SourcedEvent> notCommitedEvents = new List<SourcedEvent> ();

        public AggregateRoot () {
            Metadata = new AggregateMetadata {
                AggregateId = String.Empty,
                Exists = false,
                Version = 0
            };
        }

        public void RestoreEvent (SourcedEvent e) {
            if (Metadata.Version != 0) {
                if (Metadata.Version != e.Metadata.Version - 1)
                    throw new DungeonEventRestoreException ("Version problem.");
            }

            ExecuteEvent (e);
            Metadata.Version = e.Metadata.Version;
        }

        protected void Apply (SourcedEvent e) {
            ExecuteEvent (e);
            notCommitedEvents.Add (e);
        }

        private void ExecuteEvent (SourcedEvent e) {
            var method = this.GetType ()
                .GetMethods ()
                .Single (m =>
                    m.Name == "OnEvent" && m.GetParameters ().Length == 1 &&
                    m.GetParameters ().First ().ParameterType == e.GetType ());

            method.Invoke (this, new [] { e });
        }

        public IEnumerable<SourcedEvent> NotCommitedChanges () {
            return notCommitedEvents;
        }

        public void CommitChanges () {
            notCommitedEvents.Clear ();
        }

        public virtual void CreateNew (string id) {
            Apply (new AggregateCreatedEvent { Metadata = new SourcedEventMetadata { AggregateId = id } });
        }

        public virtual void Delete () {
            Apply (new AggregateRemovedEvent ());
        }

        public virtual void OnEvent (AggregateCreatedEvent e) {
            this.Metadata.Exists = true;
            this.Metadata.AggregateId = e.Metadata.AggregateId;
        }

        public virtual void OnEvent (AggregateRemovedEvent e) {
            this.Metadata.Exists = false;
        }
    }

    public abstract class AggregateRoot<TMemento> : AggregateRoot, ISnapshotable<TMemento> where TMemento : Memento {
        public abstract TMemento TakeSnapshot ();
        public abstract void RestoreSnapshot (TMemento memenot);
    }
}