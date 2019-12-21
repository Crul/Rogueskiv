using SDL2;
using Seedwork.Core.Components;
using System;
using System.Drawing;
using static SDL2.SDL;

namespace Seedwork.Ux.Renderers
{
    public abstract class SpriteRenderer<T> : BaseItemRenderer<T>
        where T : IComponent
    {
        protected readonly Tuple<int, int> OutputSize;
        protected readonly IntPtr Texture;
        protected SDL_Rect TextureRect;
        private readonly bool ShouldDestroyTexture;

        protected SpriteRenderer(
            UxContext uxContext,
            string imgPath,
            SDL_Rect textureRect,
            Tuple<int, int> outputSize = null
        ) : this(uxContext, textureRect, outputSize)
        {
            ShouldDestroyTexture = true;
            Texture = SDL_image.IMG_LoadTexture(UxContext.WRenderer, imgPath);
        }

        protected SpriteRenderer(
            UxContext uxContext,
            IntPtr texture,
            SDL_Rect textureRect,
            Tuple<int, int> outputSize = null
        ) : this(uxContext, textureRect, outputSize)
        {
            ShouldDestroyTexture = false;
            Texture = texture;
        }

        private SpriteRenderer(
            UxContext uxContext,
            SDL_Rect textureRect,
            Tuple<int, int> outputSize = null
        ) : base(uxContext)
        {
            TextureRect = textureRect;
            OutputSize = outputSize ?? new Tuple<int, int>(textureRect.w, textureRect.h);
        }

        protected virtual void Render(PointF position)
        {
            var screenPosition = GetScreenPosition(position, UxContext);

            var tRect = new SDL_Rect()
            {
                x = (int)screenPosition.X - OutputSize.Item1 / 2,
                y = (int)screenPosition.Y - OutputSize.Item2 / 2,
                w = OutputSize.Item1,
                h = OutputSize.Item2
            };

            SDL_RenderCopy(UxContext.WRenderer, Texture, ref TextureRect, ref tRect);
        }

        protected static PointF GetScreenPosition(PointF position, UxContext uxContext) =>
            new PointF(
                position.X + uxContext.CenterX,
                position.Y + uxContext.CenterY
            );

        protected override void Dispose(bool cleanManagedResources)
        {
            if (ShouldDestroyTexture)
                SDL_DestroyTexture(Texture);
        }
    }
}
