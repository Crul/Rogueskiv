using SDL2;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using static SDL2.SDL;

namespace Seedwork.Ux.Renderers
{
    public enum TextAlign
    {
        TOP_LEFT,
        TOP_RIGHT,
        CENTER,
        BOTTOM_LEFT,
    }

    public class TextRenderer : IDisposable
    {

        protected readonly UxContext UxContext;
        protected readonly IntPtr Font;
        private IntPtr TextureCache;
        private (string text, (byte r, byte g, byte b, byte a)) DataCache;

        public SDL_Surface SurfaceCache { get; private set; }

        public TextRenderer(UxContext uxContext, IntPtr font)
        {
            UxContext = uxContext;
            Font = font;
        }

        public void Render(
            string text,
            SDL_Color textColor,
            Point position,
            TextAlign align,
            Action<Point> renderBgr = null,
            Point? minPosition = null
        )
        {
            PreRender(text, textColor);
            renderBgr?.Invoke(position);
            Render(position, align, minPosition);
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

        private void Render(Point position, TextAlign align, Point? minPosition = null)
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
                x = position.X,
                y = position.Y,
                w = SurfaceCache.w,
                h = SurfaceCache.h
            };

            switch (align)
            {
                case TextAlign.TOP_RIGHT:
                    dest.x -= SurfaceCache.w;
                    break;
                case TextAlign.CENTER:
                    dest.x -= SurfaceCache.w / 2;
                    dest.y -= SurfaceCache.h / 2;
                    break;
                case TextAlign.BOTTOM_LEFT:
                    dest.y -= SurfaceCache.h;
                    break;
            }


            if (minPosition.HasValue)
            {
                dest.x = Math.Max(minPosition.Value.X, dest.x);
                dest.y = Math.Max(minPosition.Value.Y, dest.y);
            }

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
