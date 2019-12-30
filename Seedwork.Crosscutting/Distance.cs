using System;
using System.Drawing;

namespace Seedwork.Crosscutting
{
    public static class Distance
    {
        // https://oroboro.com/fast-approximate-distance/
        private const double FAST_DIST_MAX_FACTOR = 1007d / 1024d;
        private const double FAST_DIST_MIN_FACTOR = 441d / 1024d;

        public static float Get(PointF p1, PointF p2) => Get(p1.X - p2.X, p1.Y - p2.Y);

        private static float Get(float deltaX, float deltaY)
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
