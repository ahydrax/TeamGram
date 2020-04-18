using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using TeamGram.Teamspeak;
using TeamGram.Telegram;

namespace TeamGram.Handlers
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class UsersListAskedHandler : INotificationHandler<UserListAsked>
    {
        private readonly ITeamspeakUsersProvider _teamspeakUsersProvider;
        private readonly IMediator _mediator;

        public UsersListAskedHandler(ITeamspeakUsersProvider teamspeakUsersProvider, IMediator mediator)
        {
            _teamspeakUsersProvider = teamspeakUsersProvider;
            _mediator = mediator;
        }
        
        public async Task Handle(UserListAsked notification, CancellationToken cancellationToken)
        {
            var users = await _teamspeakUsersProvider.GetUsers(cancellationToken);
            var text = string.Join("\r\n", users);
            await _mediator.Publish(new NewTextMessage(text), cancellationToken);
        }
    }
}
