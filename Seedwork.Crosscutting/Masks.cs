using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Seedwork.Crosscutting
{
    public static class Masks
    {
        public static List<Rectangle> GetCircleMask(int radius)
        {
            var size = radius * 2;
            var img = new Bitmap(size, size);
            using (var graphics = Graphics.FromImage(img))
                graphics.FillEllipse(
                    new SolidBrush(Color.White),
                    -0.5f, -0.5f, size, size
                );

            return GetFromImage(img);
        }

        public static List<Rectangle> GetFromImage(Bitmap image)
        {
            // only working for circle images or alike

            var mask = new List<Rectangle>();
            var columns = Enumerable.Range(0, image.Width).ToList();
            var currentRowMask = new Rectangle(-1, -1, -1, -1);
            for (var y = 0; y < image.Height; y++)
            {
                var nonBlackRowPixels = columns
                    .Select(x => (x, pixel: image.GetPixel(x, y)))
                    .Where(info => info.pixel.R != 0)
                    .ToList();

                if (nonBlackRowPixels.Count == 0)
                    continue;

                var startX = nonBlackRowPixels.First().x;
                var endX = nonBlackRowPixels.Last().x;

                var isEqualThanLastRow = currentRowMask.X == startX
                    && currentRowMask.Width == (endX - startX);

                if (isEqualThanLastRow)
                {
                    currentRowMask.Height++;
                    continue;
                }

                if (currentRowMask.X > -1)
                    mask.Add(currentRowMask);

                currentRowMask = new Rectangle(
                    x: startX,
                    y: y,
                    width: endX - startX,
                    height: 1
                );
            }

            return mask;
        }
    }
}
