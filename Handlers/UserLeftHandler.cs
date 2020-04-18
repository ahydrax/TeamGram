using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using TeamGram.Teamspeak;
using TeamGram.Telegram;

namespace TeamGram.Handlers
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class UserLeftHandler : INotificationHandler<UserLeft>
    {
        private readonly IMediator _mediator;

        public UserLeftHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(UserLeft notification, CancellationToken cancellationToken)
        {
            var text = $"{notification.Username} left";
            await _mediator.Publish(new NewTextMessage(text), cancellationToken);
        }
    }
}
