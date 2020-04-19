using System;

namespace TeamGram.Configuration
{
    public class MongoConfiguration
    {
        public string? Uri { get; set; }

        [Obsolete("Exists only for configuration api", true)]
        public MongoConfiguration() { }
    }
}
