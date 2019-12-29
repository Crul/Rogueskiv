using SDL2;
using Seedwork.Crosscutting;
using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using System;
using System.Drawing;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class BgrRenderer : IRenderer
    {
        private readonly UxContext UxContext;
        private readonly IntPtr Texture;
        private readonly Size TextureSize;

        public BgrRenderer(UxContext uxContext, string texturePath, Size textureSize)
        {
            UxContext = uxContext;
            Texture = SDL_image.IMG_LoadTexture(uxContext.WRenderer, texturePath);
            TextureSize = textureSize;
        }

        public void Render()
        {
            var topLeftPoint = UxContext.Center.Substract(UxContext.ScreenSize.Divide(2).ToPoint());
            var initialScreenPos = new Point(
                x: topLeftPoint.X % TextureSize.Width,
                y: topLeftPoint.Y % TextureSize.Height
            );
            var screenPos = new Point(initialScreenPos.X, initialScreenPos.Y);

            do
            {
                var textureRect = new SDL_Rect()
                {
                    x = 0,
                    y = 0,
                    w = TextureSize.Width,
                    h = TextureSize.Height,
                };
                var outputRect = new SDL_Rect()
                {
                    x = screenPos.X,
                    y = screenPos.Y,
                    w = TextureSize.Width,
                    h = TextureSize.Height,
                };

                SDL_RenderCopy(UxContext.WRenderer, Texture, ref textureRect, ref outputRect);

                screenPos = screenPos.Add(x: TextureSize.Width);
                if (screenPos.X > UxContext.ScreenSize.Width)
                    screenPos = new Point(initialScreenPos.X, screenPos.Y + TextureSize.Height);

            } while (screenPos.Y < UxContext.ScreenSize.Height);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool cleanManagedResources)
        {
            if (cleanManagedResources)
                SDL_DestroyTexture(Texture);
        }
    }
}
