using System;

namespace Seedwork.Crosscutting
{
    public static class Distance
    {
        // https://oroboro.com/fast-approximate-distance/
        private const double FAST_DIST_MAX_FACTOR = 1007d / 1024d;
        private const double FAST_DIST_MIN_FACTOR = 441d / 1024d;

        public static float Get(float deltaX, float deltaY)
        {
            var maxDelta = Math.Abs(deltaX);
            var minDelta = Math.Abs(deltaY);
            if (maxDelta < minDelta)
                (minDelta, maxDelta) = (maxDelta, minDelta);

            return DistanceSortedParams(maxDelta, minDelta);
        }

        private static float DistanceSortedParams(float maxDelta, float minDelta)
            => (float)Math.Abs(
                FAST_DIST_MAX_FACTOR * maxDelta + FAST_DIST_MIN_FACTOR * minDelta
            );
    }
}
