using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace TeamGram.Phrases
{
    public class PhrasesProvider
    {
        private readonly IMongoDatabase _mongoDatabase;
        private readonly ILogger<PhrasesProvider> _logger;

        public PhrasesProvider([NotNull] IMongoDatabase mongoDatabase, [NotNull] ILogger<PhrasesProvider> logger)
        {
            _mongoDatabase = mongoDatabase ?? throw new ArgumentNullException(nameof(mongoDatabase));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> GetCustomGreeting(string username, CancellationToken cancellationToken = default)
        {
            var collection = _mongoDatabase.GetCollection<UserCustomPhrases>("phrases");
            var userPhrases = await collection.GetByUsername(username, cancellationToken);

            return userPhrases == null
                ? "{0} joined"
                : userPhrases.Greetings.GetRandomElementOrDefault("{0} joined");
        }

        public async Task<string> GetCustomFarewell(string username, CancellationToken cancellationToken = default)
        {
            var collection = _mongoDatabase.GetCollection<UserCustomPhrases>("phrases");
            var userPhrases = await collection.GetByUsername(username, cancellationToken);

            return userPhrases == null
                ? "{0} left"
                : userPhrases.Farewells.GetRandomElementOrDefault("{0} left");
        }

        public async Task<string> GetEmptyServerCustomPhrase(CancellationToken cancellationToken = default)
        {
            var collection = _mongoDatabase.GetCollection<EmptyServerCustomPhrase>("phrases_emptyserver");
            var phrases = await collection.GetAll(cancellationToken);
            var defaultPhrase = new EmptyServerCustomPhrase("there are no clients connected");
            return phrases.GetRandomElementOrDefault(defaultPhrase).Phrase;
        }
    }
}
