using System;
using JetBrains.Annotations;

namespace TeamGram.Configuration
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class ApplicationConfiguration
    {
        public TeamspeakConfiguration? Teamspeak { get; set; }
        public TelegramConfiguration? Telegram { get; set; }
        public ElasticsearchConfiguration? Elastic { get; set; }

        [Obsolete("Exists only for configuration api", true)]
        public ApplicationConfiguration() { }
    }
}
