using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using TeamGram.Configuration;
using TeamGram.Telegram;

namespace TeamGram.Handlers
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class CredentialsAskedHandler : INotificationHandler<CredentialsAsked>
    {
        private readonly TeamspeakConfiguration _teamspeakConfiguration;
        private readonly IMediator _mediator;

        public CredentialsAskedHandler(TeamspeakConfiguration teamspeakConfiguration,
            IMediator mediator)
        {
            _teamspeakConfiguration = teamspeakConfiguration;
            _mediator = mediator;
        }

        public async Task Handle(CredentialsAsked notification, CancellationToken cancellationToken)
        {
            var responseText = @$"
host: {_teamspeakConfiguration.Host}
password: {_teamspeakConfiguration.Password}";

            await _mediator.Publish(new NewTextMessage(responseText));
        }
    }
}
