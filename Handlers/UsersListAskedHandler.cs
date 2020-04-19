using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using TeamGram.Phrases;
using TeamGram.Teamspeak;
using TeamGram.Telegram;

namespace TeamGram.Handlers
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class UsersListAskedHandler : INotificationHandler<UserListAsked>
    {
        private readonly ITeamspeakUsersProvider _teamspeakUsersProvider;
        private readonly IMediator _mediator;
        private readonly PhrasesProvider _phrasesProvider;

        public UsersListAskedHandler(
            [NotNull] ITeamspeakUsersProvider teamspeakUsersProvider,
            [NotNull] IMediator mediator,
            [NotNull] PhrasesProvider phrasesProvider)
        {
            _teamspeakUsersProvider = teamspeakUsersProvider ?? throw new ArgumentNullException(nameof(teamspeakUsersProvider));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _phrasesProvider = phrasesProvider ?? throw new ArgumentNullException(nameof(phrasesProvider));
        }

        public async Task Handle(UserListAsked notification, CancellationToken cancellationToken)
        {
            var users = await _teamspeakUsersProvider.GetUsers(cancellationToken);
            var text = users.Length == 0
                ? await GetEmptyServerPhrase(cancellationToken)
                : string.Join("\r\n", users);
            await _mediator.Publish(new NewTextMessage(text), cancellationToken);
        }

        private async Task<string> GetEmptyServerPhrase(CancellationToken cancellationToken)
            => await _phrasesProvider.GetEmptyServerCustomPhrase(cancellationToken);
    }
}
