using SDL2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Seedwork.Ux.MediaProviders
{
    class FontProvider
    {
        private readonly string FontsPath;
        private readonly IDictionary<(string path, int size), IntPtr> FontsByPathAndSize;

        public FontProvider(string fontsPath)
        {
            FontsPath = fontsPath;
            FontsByPathAndSize = new Dictionary<(string, int), IntPtr>();
        }

        public IntPtr GetFont(string fontFile, int fontSize)
        {
            var fontPath = Path.Combine(FontsPath, fontFile);
            if (!FontsByPathAndSize.TryGetValue((fontPath, fontSize), out var font))
            {
                font = SDL_ttf.TTF_OpenFont(fontFile, fontSize);
                FontsByPathAndSize[(fontPath, fontSize)] = font;
            }

            return font;
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
                FontsByPathAndSize.Values.ToList().ForEach(SDL_ttf.TTF_CloseFont);
                FontsByPathAndSize.Clear();
            }
        }
    }
}
