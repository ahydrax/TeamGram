using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MihaZupan;
using TeamGram.Configuration;
using TeamGram.Services.Telegram.Events;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace TeamGram.Services.Telegram
{
    public class TelegramMessagingService : ITelegramMessageSender, IHostedService, IDisposable
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

            if (message.Chat.Id != _telegramConfiguration.HostGroupId)
            {
                _logger.LogInformation("Skipping message '{message}' from unknown chat {chatId} ({chatUsername})",
                    message.Text, message.Chat.Id, message.Chat.Username);
                return;
            }

            var messageText = message.Text;

            switch (messageText)
            {
                case "/whots":
                    _mediator.Publish(new WhoTsAsked());
                    break;
            }
        }

        private void LogGeneralError(object? sender, ReceiveGeneralErrorEventArgs e)
            => _logger.LogError(e.Exception, "Receive error occured: {errorMessage}", e.Exception.Message);

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _telegramBotClient.StartReceiving(cancellationToken: cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _telegramBotClient.StopReceiving();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_telegramBotClient.IsReceiving)
            {
                _telegramBotClient.StopReceiving();
            }

            _telegramHttpClient.Dispose();
        }

        public async Task SendMessage(string text)
        {
            var chatId = new ChatId(_telegramConfiguration.HostGroupId);
            var formattedText = string.Concat("```", text, "```");

            await _telegramBotClient.SendTextMessageAsync(chatId, formattedText, disableNotification: true);
        }
    }
}
