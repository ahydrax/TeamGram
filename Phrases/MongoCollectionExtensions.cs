using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace TeamGram.Phrases
{
    public static class MongoCollectionExtensions
    {
        public static async Task<UserCustomPhrases> GetByUsername(
            this IMongoCollection<UserCustomPhrases> collection,
            string username,
            CancellationToken cancellationToken = default)
        {
            var cursor = await collection.FindAsync(
                x => x.Id == username,
                new FindOptions<UserCustomPhrases>
                {
                    BatchSize = 1
                }, cancellationToken: cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }

        public static async Task<EmptyServerCustomPhrase[]> GetAll(
            this IMongoCollection<EmptyServerCustomPhrase> collection,
            CancellationToken cancellationToken = default)
        {
            var cursor = await collection.FindAsync(
                x => true,
                cancellationToken: cancellationToken);

            var result = await cursor.ToListAsync(cancellationToken);

            return result.ToArray();
        }
    }
}
