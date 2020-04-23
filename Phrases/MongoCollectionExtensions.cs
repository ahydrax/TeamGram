using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace TeamGram.Phrases
{
    public static class MongoCollectionExtensions
    {
        public static IMongoCollection<CustomPhrase> CustomPhrases(
            this IMongoDatabase database)
            => database.GetCollection<CustomPhrase>("phrases");

        public static Task<IReadOnlyCollection<TConcrete>> GetAll<TAbstract, TConcrete>(
            this IMongoCollection<TAbstract> collection,
            CancellationToken cancellationToken = default)
            where TConcrete : TAbstract
            => GetAll(collection, (TConcrete x) => true, cancellationToken);

        public static async Task<IReadOnlyCollection<TConcrete>> GetAll<TAbstract, TConcrete>(
            this IMongoCollection<TAbstract> collection,
            Expression<Func<TConcrete, bool>> selector,
            CancellationToken cancellationToken = default)
            where TConcrete : TAbstract
        {
            var cursor = await collection.OfType<TConcrete>().FindAsync(selector, cancellationToken: cancellationToken);
            return await cursor.ToListAsync(cancellationToken);
        }
    }
}
