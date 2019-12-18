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

        protected TextRenderer(UxContext uxContext, IntPtr font)
            : base(uxContext) => Font = font;

        protected override void Render(IEntity entity, float interpolation)
        {
            var component = entity.GetComponent<T>();
            var text = GetText(component);
            var textColor = GetColor(component);
            (var x, var y) = GetPosition(component);

            // TODO cache and reuse rendered text if not changed
            var renderedText = SDL_ttf.TTF_RenderText_Blended(Font, text, textColor);
            var surface = (SDL_Surface)Marshal.PtrToStructure(renderedText, typeof(SDL_Surface));
            var texturedText = SDL_CreateTextureFromSurface(UxContext.WRenderer, renderedText);

            var src = new SDL_Rect()
            {
                x = 0,
                y = 0,
                w = surface.w,
                h = surface.h
            };
            var dest = new SDL_Rect()
            {
                x = x,
                y = y,
                w = surface.w,
                h = surface.h
            };

            SDL_RenderCopy(UxContext.WRenderer, texturedText, ref src, ref dest);
            SDL_DestroyTexture(texturedText);
            SDL_FreeSurface(renderedText);
        }

        protected abstract string GetText(T component);
        protected abstract SDL_Color GetColor(T component);
        protected abstract (int x, int y) GetPosition(T component);
    }
}
