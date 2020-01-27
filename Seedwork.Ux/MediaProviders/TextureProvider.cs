using SDL2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static SDL2.SDL;

namespace Seedwork.Ux.MediaProviders
{
    class TextureProvider : IDisposable
    {
        private readonly IntPtr WRenderer;
        private readonly string ImagesPath;
        private readonly IDictionary<string, IntPtr> TexturesByPath;

        public TextureProvider(IntPtr wRenderer, string imagesPath)
        {
            WRenderer = wRenderer;
            ImagesPath = imagesPath;
            TexturesByPath = new Dictionary<string, IntPtr>();
        }

        public IntPtr GetTexture(string imageFile)
        {
            var imagePath = Path.Combine(ImagesPath, imageFile);
            if (!TexturesByPath.TryGetValue(imagePath, out var texture))
            {
                texture = SDL_image.IMG_LoadTexture(WRenderer, imagePath);
                TexturesByPath[imagePath] = texture;
            }

            return texture;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool cleanManagedResources)
        {
            if (cleanManagedResources)
            {
                TexturesByPath.Values.ToList().ForEach(SDL_DestroyTexture);
                TexturesByPath.Clear();
            }
        }
    }
}
