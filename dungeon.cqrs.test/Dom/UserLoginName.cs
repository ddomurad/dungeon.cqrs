using dungeon.cqrs.core.event_sourcing.models;
using dungeon.cqrs.event_sourcing;
using dungeon.cqrs.implementation.event_sourcing;
using dungeon.cqrs.test.Models;

namespace dungeon.cqrs.test.Dom {

    public class UserLoginMemoento : Memento {
        public string UserId { get; set; }
    }

    public class UserLoginName : AggregateRoot<UserLoginMemoento> {
        public string UserId { get; private set; }

        public void ReserverFor (string userId) {
            Apply (new LoginReservedEvent { UserId = userId });
        }

        public void OnEvent (LoginReservedEvent e) {
            UserId = e.UserId;
        }

        public override UserLoginMemoento TakeSnapshot () {
            return new UserLoginMemoento {
                UserId = UserId,
                Metadata = (AggregateMetadata) this.Metadata.Clone()
            };
        }

        public override void RestoreSnapshot (UserLoginMemoento memenot) {
            this.Metadata = (AggregateMetadata)memenot.Metadata.Clone();
            this.UserId = memenot.UserId;
        }
    }
}