using Rogueskiv.Core.Components;
using SDL2;
using Seedwork.Core.Entities;
using Seedwork.Ux;
using Seedwork.Ux.Renderers;

namespace Rogueskiv.Ux.Renderers
{
    class HealthRenderer : BaseItemRenderer<HealthComp>
    {
        private const int X_LEFT_POS = 20;
        private const int Y_BOTTOM_POS = 20;
        private const int MAX_WIDTH = 200;
        private const int HEIGHT = 20;

        public HealthRenderer(UxContext uxContext)
            : base(uxContext) { }

        protected override void Render(IEntity entity, float interpolation)
        {
            SDL.SDL_GetRenderDrawColor(UxContext.WRenderer, out byte r, out byte g, out byte b, out byte a);

            var healthComp = entity.GetComponent<HealthComp>();
            var healthBarWidht = (int)(MAX_WIDTH * healthComp.HealthFactor);
            var yPos = UxContext.ScreenHeight - Y_BOTTOM_POS - HEIGHT;

            RenderRect(yPos, MAX_WIDTH, red: 0x99);
            RenderRect(yPos, healthBarWidht, green: 0x99);

            SDL.SDL_SetRenderDrawColor(UxContext.WRenderer, r, g, b, a);
        }

        private void RenderRect(
            int yPos, int width, byte red = 0x00, byte green = 0x00, byte blue = 0x00
        )
        {
            var rect = new SDL.SDL_Rect()
            {
                x = X_LEFT_POS,
                y = yPos,
                w = width,
                h = HEIGHT
            };
            SDL.SDL_SetRenderDrawColor(UxContext.WRenderer, red, green, blue, 0xFF);
            SDL.SDL_RenderFillRect(UxContext.WRenderer, ref rect);
        }
    }
}
