using System;

namespace Seedwork.Crosscutting
{
    public static class Luck
    {
        private static Random Random = new Random();

        public static int Reset(int? seed = null)
        {
            var seedValue = seed ?? Random.Next();
            SetSeed(seedValue);

            return seedValue;
        }

        public static void SetSeed(int seed) => Random = new Random(seed);
        public static int Next(int maxValue) => Random.Next(maxValue);
        public static int Next(int minValue, int maxValue) => Random.Next(minValue, maxValue);
        public static double NextDouble() => Random.NextDouble();
    }
}
