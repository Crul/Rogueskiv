using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Seedwork.Crosscutting
{
    public static class Masks
    {
        public static List<Rectangle> GetFromImage(string imagePath)
        {
            // only working for circle images or alike

            using var stream = File.OpenRead(imagePath);
            var image = Image.Load<Rgba32>(stream);
            var imageSize = image.Size();

            var mask = new List<Rectangle>();
            var columns = Enumerable.Range(0, imageSize.Width).ToList();
            var currentRowMask = new Rectangle(-1, -1, -1, -1);
            for (var y = 0; y < imageSize.Height; y++)
            {
                var nonBlackRowPixels = columns
                    .Select(x => (x, pixel: image[x, y]))
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
