using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MihaZupan;
using TeamGram.Configuration;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TeamGram.Telegram
{
    public class TelegramMessagingService : ITelegramMessageSender, IHostedService
    {
        private readonly TelegramConfiguration _telegramConfiguration;
        private readonly IMediator _mediator;
        private readonly ILogger<TelegramMessagingService> _logger;
        private readonly TelegramBotClient _telegramBotClient;
        private readonly HttpClient _telegramHttpClient;

        public TelegramMessagingService(TelegramConfiguration telegramConfiguration,
            IMediator mediator,
            ILogger<TelegramMessagingService> logger)
        {
            _telegramConfiguration = telegramConfiguration;
            _mediator = mediator;
            _logger = logger;

            var proxy = new HttpToSocks5Proxy(
                telegramConfiguration.Socks5Host,
                telegramConfiguration.Socks5Port,
                telegramConfiguration.Socks5Username,
                telegramConfiguration.Socks5Password)
            {
                ResolveHostnamesLocally = true
            };
            var handler = new HttpClientHandler
            {
                Proxy = proxy,
                UseProxy = true
            };
            _telegramHttpClient = new HttpClient(handler);
            _telegramBotClient = new TelegramBotClient(telegramConfiguration.BotApiKey, _telegramHttpClient);
            _telegramBotClient.OnMessage += ProcessMessage;
            _telegramBotClient.OnReceiveGeneralError += LogGeneralError;
        }

        private void ProcessMessage(object? sender, MessageEventArgs e)
        {
            var message = e.Message;
            var messageText = message.Text;

            if (message.Chat.Id != _telegramConfiguration.HostGroupId)
            {
                _logger.LogInformation("Skipping message {message} from unknown chat {chatId} ({chatUsername})",
                    messageText, message.Chat.Id, message.Chat.Username);
                return;
            }

            _logger.LogInformation("Incoming message {message} from host chat", messageText);

            var botHighlightPart = $"@{_telegramConfiguration.BotUsername}";
            var commandText = messageText.Replace(botHighlightPart, string.Empty);

            switch (commandText)
            {
                case "/users":
                    _mediator.Publish(new UserListAsked());
                    break;

                case "/credentials":
                    _mediator.Publish(new CredentialsAsked());
                    break;
            }
        }

        private void LogGeneralError(object? sender, ReceiveGeneralErrorEventArgs e)
            => _logger.LogError(e.Exception, "Receive error occured: {errorMessage}", e.Exception.Message);

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _telegramBotClient.StartReceiving(cancellationToken: cancellationToken);
            _logger.LogInformation("Telegram message receiving started");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _telegramBotClient.StopReceiving();
            _logger.LogInformation("Telegram message receiving stopped");
            _telegramHttpClient.Dispose();
            return Task.CompletedTask;
        }

        public async Task SendMessage(string text, CancellationToken cancellationToken = default)
        {
            var chatId = new ChatId(_telegramConfiguration.HostGroupId);

            var formattedText = text.Contains("\r\n")
                ? $"```\r\n{text}```"
                : $"`{text}`";

            await _telegramBotClient.SendTextMessageAsync(chatId, formattedText, ParseMode.Markdown, disableNotification: true,
                cancellationToken: cancellationToken);
        }
    }
}
