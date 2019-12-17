using System;

namespace Seedwork.Crosscutting
{
    public static class Luck
    {
        private static Random Random = new Random();

        public static int Next(int maxValue) => Random.Next(maxValue);
    }
}
