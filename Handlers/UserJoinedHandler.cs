using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using TeamGram.Teamspeak;
using TeamGram.Telegram;

namespace TeamGram.Handlers
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class UserJoinedHandler : INotificationHandler<UserJoined>
    {
        private readonly IMediator _mediator;

        public UserJoinedHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(UserJoined notification, CancellationToken cancellationToken)
        {
            var text = $"{notification.Username} joined";
            await _mediator.Publish(new NewTextMessage(text), cancellationToken);
        }
    }
}
