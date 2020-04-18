using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Logging;
using TeamGram.Telegram;

namespace TeamGram.Handlers
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class NewTextMessageHandler : INotificationHandler<NewTextMessage>
    {
        private readonly ITelegramMessageSender _telegramMessageSender;
        private readonly ILogger<NewTextMessageHandler> _logger;

        public NewTextMessageHandler(ITelegramMessageSender telegramMessageSender, ILogger<NewTextMessageHandler> logger)
        {
            _telegramMessageSender = telegramMessageSender;
            _logger = logger;
        }

        public async Task Handle(NewTextMessage notification, CancellationToken cancellationToken)
        {
            var messageText = notification.MessageText;
            _logger.LogInformation("Message sent {messageText}", messageText);
            await _telegramMessageSender.SendMessage(messageText, cancellationToken);
        }
    }
}
