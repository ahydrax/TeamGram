using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using MihaZupan;
using Telegram.Bot;

namespace TeamGram.Configuration
{
    public class TelegramConfiguration
    {
        public string BotApiKey { get; set; }
        public long HostGroupId { get; set; }
        public string Socks5Host { get; set; }
        public int Socks5Port { get; set; }
        public string Socks5Username { get; set; }
        public string Socks5Password { get; set; }
    }

    public static class TelegramConfigurationExtensions
    {
        public static IServiceCollection AddTelegram(this IServiceCollection services, TelegramConfiguration configuration)
        {

            services.AddSingleton(telegramBotClient);

            return services;
        }
    }
}
