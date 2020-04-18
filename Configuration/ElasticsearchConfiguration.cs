using System;
using JetBrains.Annotations;

namespace TeamGram.Configuration
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class ElasticsearchConfiguration
    {
        public string? Uri { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }

        [Obsolete("Exists only for configuration api", true)]
        public ElasticsearchConfiguration() { }
    }
}
