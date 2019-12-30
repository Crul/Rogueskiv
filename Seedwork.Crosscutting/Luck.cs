using System;

namespace Seedwork.Crosscutting
{
    public static class Luck
    {
        private static Random Random = new Random();

        public static int Reset()
        {
            var seed = Random.Next();
            SetSeed(seed);

            return seed;
        }

        public static void SetSeed(int seed) => Random = new Random(seed);
        public static int Next(int maxValue) => Random.Next(maxValue);
        public static int Next(int minValue, int maxValue) => Random.Next(minValue, maxValue);
        public static double NextDouble() => Random.NextDouble();
    }
}
