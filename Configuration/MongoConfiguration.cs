using System;

namespace TeamGram.Configuration
{
    public class MongoConfiguration
    {
        public string? Uri { get; set; }
        public string? Database { get; set; }

        [Obsolete("Exists only for configuration api", true)]
        public MongoConfiguration() { }
    }
}
