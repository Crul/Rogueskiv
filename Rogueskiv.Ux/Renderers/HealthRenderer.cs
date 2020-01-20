using Rogueskiv.Core.Components;
using Seedwork.Core.Entities;
using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using System;
using System.Drawing;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class HealthRenderer : CompRenderer<HealthComp>
    {
        private const int X_LEFT_POS = 36;
        private const int Y_BOTTOM_POS = 36;
        private const int MAX_BAR_WIDTH = 220;
        private const int BAR_HEIGHT = 30;
        private const int TEXTURE_WIDTH = 240;
        private const int TEXTURE_HEIGHT = 34;

        private readonly IntPtr Texture;
        private SDL_Rect TextureRect;
        private Size TextureSize;

        public HealthRenderer(UxContext uxContext)
            : base(uxContext)
        {
            Texture = uxContext.GetTexture("player-health-bar.png");
            TextureSize = new Size(TEXTURE_WIDTH, TEXTURE_HEIGHT);
            TextureRect = new SDL_Rect() { x = 0, y = 0, w = TextureSize.Width, h = TextureSize.Height };
        }

        protected override void Render(IEntity entity, HealthComp healthComp, float interpolation)
        {
            SDL_GetRenderDrawColor(UxContext.WRenderer, out byte r, out byte g, out byte b, out byte a);

            var healthBarWidht = (int)(MAX_BAR_WIDTH * healthComp.HealthFactor);
            var yPos = UxContext.ScreenSize.Height - Y_BOTTOM_POS - BAR_HEIGHT;

            var barYPos = yPos + (TEXTURE_HEIGHT - BAR_HEIGHT) / 2;
            RenderRect(barYPos, MAX_BAR_WIDTH, red: 0x99);
            RenderRect(barYPos, healthBarWidht, green: 0x99);

            SDL_SetRenderDrawColor(UxContext.WRenderer, r, g, b, a);

            var outputRect = new SDL_Rect()
            {
                x = X_LEFT_POS,
                y = yPos,
                w = TextureSize.Width,
                h = TextureSize.Height
            };
            SDL_RenderCopy(UxContext.WRenderer, Texture, ref TextureRect, ref outputRect);
        }

        private void RenderRect(
            int yPos, int width, byte red = 0x00, byte green = 0x00, byte blue = 0x00
        )
        {
            var rect = new SDL_Rect()
            {
                x = X_LEFT_POS + (TEXTURE_WIDTH - MAX_BAR_WIDTH) / 2,
                y = yPos,
                w = width,
                h = BAR_HEIGHT
            };
            SDL_SetRenderDrawColor(UxContext.WRenderer, red, green, blue, 0xFF);
            SDL_RenderFillRect(UxContext.WRenderer, ref rect);
        }
    }
}
