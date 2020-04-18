using System;
using JetBrains.Annotations;

namespace TeamGram.Configuration
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class TeamspeakConfiguration
    {
        public string? Host { get; set; }
        public int Port { get; set; }
        public string? Username { get; set; }
        public string? Key { get; set; }
        public string? Password { get; set; }

        [Obsolete("Exists only for configuration api", true)]
        public TeamspeakConfiguration() { }
    }
}
