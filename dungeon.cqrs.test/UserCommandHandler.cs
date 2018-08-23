using System;
using System.Threading.Tasks;
using dungeon.cqrs.core.commands;
using dungeon.cqrs.core.event_sourcing;
using dungeon.cqrs.test.Dom;
using dungeon.cqrs.test.Models;

namespace dungeon.cqrs.test
{
    public class UserCommandHandler:
        ICommandHandler<Guid, RegisterUserCommand>,
        ICommandHandler<RemoveUserCommand> {
            private IEventSource eventSource { get; }
            public UserCommandHandler (IEventSource eventSource) {
                this.eventSource = eventSource;
            }

            public async Task<Guid> Handle (RegisterUserCommand command) {
                var login = await eventSource.CreateNew<UserLoginName> (command.Login);
                var userId = Guid.NewGuid ();
                
                login.ReserverFor (userId.ToString ());

                var user = await eventSource.CreateNew<UserDom> (userId.ToString ());

                await eventSource.PushChanges (command);
                return userId;
            }

            public async Task Handle (RemoveUserCommand command) {
                var login = await eventSource.Pull<UserLoginName> (command.Login);
                var user = await eventSource.Pull<UserDom>(login.UserId);
                
                login.Delete();
                user.Delete();

                await eventSource.PushChanges (command);
            }
        }
}