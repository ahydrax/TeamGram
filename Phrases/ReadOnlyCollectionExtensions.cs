using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TeamGram.Phrases
{
    public static class ReadOnlyCollectionExtensions
    {
        private static readonly ThreadLocal<Random> ThreadLocalRandom
            = new ThreadLocal<Random>(() => new Random());

        private static Random Random => ThreadLocalRandom.Value!;

        public static T GetRandomElementOrDefault<T>(this IReadOnlyCollection<T> collection, T defaultValue)
        {
            if (collection.Count == 0) return defaultValue;
            var randomIndex = Random.Next(0, collection.Count);
            return collection.ElementAt(randomIndex);
        }
    }
}
