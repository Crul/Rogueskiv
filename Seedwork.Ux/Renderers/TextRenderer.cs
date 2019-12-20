using SDL2;
using Seedwork.Core.Components;
using Seedwork.Core.Entities;
using System;
using System.Runtime.InteropServices;
using static SDL2.SDL;

namespace Seedwork.Ux.Renderers
{
    public abstract class TextRenderer<T> : BaseItemRenderer<T>
        where T : IComponent
    {
        private readonly IntPtr Font;
        private (string text, (byte r, byte g, byte b, byte a), (int x, int y)) DataCache;
        private SDL_Surface SurfaceCache;
        private IntPtr TextureCache;

        protected TextRenderer(UxContext uxContext, IntPtr font)
            : base(uxContext) => Font = font;

        protected override void Render(IEntity entity, float interpolation)
        {
            var component = entity.GetComponent<T>();
            var text = GetText(component);
            var textColor = GetColor(component);
            var position = GetPosition(component);

            if (HasDataChanged(text, textColor, position))
            {
                SetCache(text, textColor, position);
                SDL_DestroyTexture(TextureCache);
                var renderedText = SDL_ttf.TTF_RenderText_Blended(Font, text, textColor);
                SurfaceCache = (SDL_Surface)Marshal.PtrToStructure(renderedText, typeof(SDL_Surface));
                TextureCache = SDL_CreateTextureFromSurface(UxContext.WRenderer, renderedText);
                SDL_FreeSurface(renderedText);
            }

            var src = new SDL_Rect()
            {
                x = 0,
                y = 0,
                w = SurfaceCache.w,
                h = SurfaceCache.h
            };
            var dest = new SDL_Rect()
            {
                x = position.x,
                y = position.y,
                w = SurfaceCache.w,
                h = SurfaceCache.h
            };

            SDL_RenderCopy(UxContext.WRenderer, TextureCache, ref src, ref dest);
        }

        protected abstract string GetText(T component);
        protected abstract SDL_Color GetColor(T component);
        protected abstract (int x, int y) GetPosition(T component);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            SDL_DestroyTexture(TextureCache);
        }

        private bool HasDataChanged(string text, SDL_Color color, (int, int) position) =>
            DataCache != (text, (color.r, color.g, color.b, color.a), position);

        private void SetCache(string text, SDL_Color color, (int, int) position) =>
            DataCache = (text, (color.r, color.g, color.b, color.a), position);
    }
}
