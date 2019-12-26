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

        public static PointF Add(this PointF pointF, float x = 0, float y = 0)
            => new PointF(pointF.X + x, pointF.Y + y);

        public static Point Add(this Point point1, Point point2)
            => new Point(point1.X + point2.X, point1.Y + point2.Y);

        public static Point Add(this Point point, int x = 0, int y = 0)
            => new Point(point.X + x, point.Y + y);

        public static PointF Substract(this PointF pointF1, PointF pointF2)
            => new PointF(pointF1.X - pointF2.X, pointF1.Y - pointF2.Y);

        public static Point Substract(this Point point1, Point point2)
            => new Point(point1.X - point2.X, point1.Y - point2.Y);

        public static Size Substract(this Size size1, Size size2)
            => new Size(size1.Width - size2.Width, size1.Height - size2.Height);

        public static PointF Multiply(this PointF point, float value)
            => new PointF(point.X * value, point.Y * value);

        public static PointF Multiply(this PointF point, float x = 1, float y = 1)
            => new PointF(point.X * x, point.Y * y);

        public static PointF Multiply(this Point point, int value)
            => new PointF(point.X * value, point.Y * value);

        public static Size Multiply(this Size size, float value)
            => new Size((int)(size.Width * value), (int)(size.Height * value));

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
