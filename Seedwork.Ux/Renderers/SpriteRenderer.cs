using SDL2;
using Seedwork.Core.Components;
using System;

namespace Seedwork.Ux.Renderers
{
    public abstract class SpriteRenderer<T> : BaseItemRenderer<T>
        where T : IComponent
    {
        protected readonly Tuple<int, int> OutputSize;
        protected readonly IntPtr Texture;
        protected SDL.SDL_Rect TextureRect;

        public SpriteRenderer(
            UxContext uxContext,
            string imgPath,
            SDL.SDL_Rect textureRect,
            Tuple<int, int> outputSize = null
        ) : base(uxContext)
        {
            Texture = SDL_image.IMG_LoadTexture(UxContext.WRenderer, imgPath);
            TextureRect = textureRect;
            OutputSize = outputSize ?? new Tuple<int, int>(textureRect.w, textureRect.h);
        }

        protected virtual void Render(double posX, double posY)
        {
            var x = GetPositionComponent(posX, UxContext.CenterX);
            var y = GetPositionComponent(posY, UxContext.CenterY);

            var tRect = new SDL.SDL_Rect()
            {
                x = x - OutputSize.Item1 / 2,
                y = y - OutputSize.Item2 / 2,
                w = OutputSize.Item1,
                h = OutputSize.Item2
            };

            SDL.SDL_RenderCopy(UxContext.WRenderer, Texture, ref TextureRect, ref tRect);
        }

        protected static int GetPositionComponent(double positionComponent, int windowCenter) =>
            (int)(positionComponent * UxContext.Zoom) + windowCenter;

        protected override void Dispose(bool cleanManagedResources) =>
            SDL.SDL_DestroyTexture(Texture);
    }
}
