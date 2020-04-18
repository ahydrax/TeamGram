using System;
using JetBrains.Annotations;

namespace TeamGram.Configuration
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class TelegramConfiguration
    {
        public string? BotApiKey { get; set; }
        public string? BotUsername { get; set; }
        public long HostGroupId { get; set; }
        public string? Socks5Host { get; set; }
        public int Socks5Port { get; set; }
        public string? Socks5Username { get; set; }
        public string? Socks5Password { get; set; }

        [Obsolete("Exists only for configuration api", true)]
        public TelegramConfiguration() { }
    }
}
