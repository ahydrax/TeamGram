using System;
using System.Threading;

namespace TeamGram.Phrases
{
    public static class ArrayExtensions
    {
        private static readonly ThreadLocal<Random> ThreadLocalRandom
            = new ThreadLocal<Random>(() => new Random());

        private static Random Random => ThreadLocalRandom.Value!;

        public static T GetRandomElementOrDefault<T>(this T[] array, T defaultValue)
        {
            if (array.Length == 0) return defaultValue;

            var randomIndex = Random.Next(0, array.Length);
            return array[randomIndex];
        }
    }
}
