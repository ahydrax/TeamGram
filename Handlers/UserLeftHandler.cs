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
    public class UserLeftHandler : INotificationHandler<UserLeft>
    {
        private readonly IMediator _mediator;
        private readonly PhrasesProvider _phrasesProvider;

        public UserLeftHandler(
            [NotNull] IMediator mediator,
            [NotNull] PhrasesProvider phrasesProvider)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _phrasesProvider = phrasesProvider ?? throw new ArgumentNullException(nameof(phrasesProvider));
        }

        public async Task Handle(UserLeft notification, CancellationToken cancellationToken)
        {
            var greeting = await _phrasesProvider.GetCustomFarewell(notification.Username, cancellationToken);
            var text = string.Format(greeting, notification.Username);
            await _mediator.Publish(new NewTextMessage(text), cancellationToken);
        }
    }
}
