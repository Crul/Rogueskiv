using SDL2;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using static SDL2.SDL;

namespace Seedwork.Ux.Renderers
{
    public class TextRenderer : IDisposable
    {
        private readonly UxContext UxContext;
        private readonly IntPtr Font;
        private IntPtr TextureCache;
        private (string text, (byte r, byte g, byte b, byte a)) DataCache;

        public SDL_Surface SurfaceCache { get; private set; }

        public TextRenderer(UxContext uxContext, IntPtr font)
        {
            UxContext = uxContext;
            Font = font;
        }

        public void Render(
            string text, SDL_Color textColor, Point position, Action<Point> renderBgr = null
        )
        {
            PreRender(text, textColor);
            renderBgr?.Invoke(position);
            Render(position);
        }

        private void PreRender(string text, SDL_Color textColor)
        {
            if (!HasDataChanged(text, textColor))
                return;

            SetCache(text, textColor);

            SDL_DestroyTexture(TextureCache);
            var renderedText = SDL_ttf.TTF_RenderText_Blended(Font, text, textColor);
            SurfaceCache = (SDL_Surface)Marshal.PtrToStructure(renderedText, typeof(SDL_Surface));
            TextureCache = SDL_CreateTextureFromSurface(UxContext.WRenderer, renderedText);
            SDL_FreeSurface(renderedText);
        }

        private void Render(Point position)
        {
            // TODO TextSpriteProvider ?
            var src = new SDL_Rect()
            {
                x = 0,
                y = 0,
                w = SurfaceCache.w,
                h = SurfaceCache.h
            };
            var dest = new SDL_Rect()
            {
                x = position.X - SurfaceCache.w / 2,
                y = position.Y - SurfaceCache.h / 2,
                w = SurfaceCache.w,
                h = SurfaceCache.h
            };

            SDL_RenderCopy(UxContext.WRenderer, TextureCache, ref src, ref dest);
        }

        private bool HasDataChanged(string text, SDL_Color color) =>
            DataCache != (text, (color.r, color.g, color.b, color.a));

        private void SetCache(string text, SDL_Color color) =>
            DataCache = (text, (color.r, color.g, color.b, color.a));

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool cleanManagedResources)
        {
            if (cleanManagedResources)
                SDL_DestroyTexture(TextureCache);
        }
    }
}
