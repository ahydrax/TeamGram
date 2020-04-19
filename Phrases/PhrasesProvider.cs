using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MongoDB.Driver;

namespace TeamGram.Phrases
{
    public class PhrasesProvider
    {
        private readonly IMongoDatabase _mongoDatabase;

        public PhrasesProvider([NotNull] IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase ?? throw new ArgumentNullException(nameof(mongoDatabase));
        }

        public async Task<string> GetCustomGreeting(string username, CancellationToken cancellationToken = default)
        {
            var phrases = await _mongoDatabase.CustomPhrases()
                .GetAll((UserGreetingPhrase x) => x.Username == username || x.Username == "*", cancellationToken);

            return phrases.Select(x => x.Template).ToList().GetRandomElementOrDefault("{0} joined");
        }

        public async Task<string> GetCustomFarewell(string username, CancellationToken cancellationToken = default)
        {
            var phrases = await _mongoDatabase.CustomPhrases()
                .GetAll((UserFarewellPhrase x) => x.Username == username || x.Username == "*", cancellationToken);

            return phrases.Select(x => x.Template).ToList().GetRandomElementOrDefault("{0} left");
        }

        public async Task<string> GetEmptyServerCustomPhrase(CancellationToken cancellationToken = default)
        {
            var phrases = await _mongoDatabase.CustomPhrases()
                .GetAll((ServerIsEmptyPhrase x) => true, cancellationToken);

            return phrases.Select(x => x.Text).ToList().GetRandomElementOrDefault("there are no clients connected");
        }
    }
}
