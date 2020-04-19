using System;
using JetBrains.Annotations;
using MongoDB.Bson.Serialization.Attributes;

namespace TeamGram.Phrases
{
    public class UserCustomPhrases
    {
        [BsonId]
        public string Id { get; private set; }

        [BsonIgnore]
        public string Username => Id;

        public string[] Greetings { get; private set; }

        public string[] Farewells { get; private set; }

        public UserCustomPhrases([NotNull] string username)
        {
            Id = username ?? throw new ArgumentNullException(nameof(username));
            Greetings = Array.Empty<string>();
            Farewells = Array.Empty<string>();
        }
    }
}