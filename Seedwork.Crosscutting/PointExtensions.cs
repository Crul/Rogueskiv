using System;
using System.Drawing;

namespace Seedwork.Crosscutting
{
    public static class PointExtensions
    {
        public static PointF Add(this PointF pointF, int value)
            => new PointF(pointF.X + value, pointF.Y + value);

        public static PointF Add(this PointF pointF1, PointF pointF2)
            => new PointF(pointF1.X + pointF2.X, pointF1.Y + pointF2.Y);

        public static PointF Add(this PointF pointF, float valueX, float valueY)
            => new PointF(pointF.X + valueX, pointF.Y + valueY);

        public static Point Add(this Point point1, Point point2)
            => new Point(point1.X + point2.X, point1.Y + point2.Y);

        public static PointF Substract(this PointF pointF1, PointF pointF2)
            => new PointF(pointF1.X - pointF2.X, pointF1.Y - pointF2.Y);

        public static Point Substract(this Point point1, Point point2)
            => new Point(point1.X - point2.X, point1.Y - point2.Y);

        public static Point Substract(this Point point, int valueX, int valueY)
            => new Point(point.X - valueX, point.Y - valueY);

        public static PointF Multiply(this PointF point, float value)
            => new PointF(point.X * value, point.Y * value);

        public static PointF Multiply(this PointF point, float valueX, float valueY)
            => new PointF(point.X * valueX, point.Y * valueY);

        public static PointF Multiply(this Point point, int value)
            => new PointF(point.X * value, point.Y * value);

        public static Point Divide(this PointF pointF, int value)
            => new Point((int)Math.Floor(pointF.X / value), (int)Math.Floor(pointF.Y / value));

        public static Size Divide(this Size size, int value)
            => new Size(size.Width / value, size.Height / value);

        public static Point Divide(this Point point, int value)
            => new Point(point.X / value, point.Y / value);

        public static Point ToPoint(this PointF pointF)
            => new Point((int)pointF.X, (int)pointF.Y);

        public static Point ToPoint(this Size size)
            => new Point(size.Width, size.Height);

        public static PointF Map(this PointF point1, Func<float, float> mapFn)
            => new PointF(mapFn(point1.X), mapFn(point1.Y));
    }
}
