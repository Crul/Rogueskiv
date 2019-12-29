using Rogueskiv.Core;
using Rogueskiv.Core.Components;
using SDL2;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Crosscutting;
using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using System;
using System.Drawing;

namespace Rogueskiv.Ux.Renderers
{
    class PopUpRenderer : TextRenderer<PopUpComp>
    {
        private const int PADDING = 20;
        private const byte BGR_OPACITY = 0xF0;
        private readonly IRenderizable Game;

        public PopUpRenderer(UxContext uxContext, IRenderizable game, IntPtr font)
            : base(uxContext, font) => Game = game;

        protected override void Render(IEntity entity, float interpolation)
        {
            if (!Game.Pause)
                return;

            base.Render(entity, interpolation);
        }

        protected override SDL.SDL_Color GetColor(PopUpComp component) =>
            new SDL.SDL_Color() { r = 0xFF, g = 0xFF, b = 0xFF, a = 0xFF };

        protected override Point GetPosition(PopUpComp component) =>
            UxContext.ScreenSize.Divide(2).ToPoint();

        protected override string GetText(PopUpComp component)
        {
            var resultCode = Game.Result?.ResultCode;

            if (resultCode == RogueskivGameResults.DeathResult.ResultCode)
                return "YOU'RE DEAD";

            if (resultCode == RogueskivGameResults.WinResult.ResultCode)
                return "YOU WIN";

            return "PAUSE";
        }

        protected override void RenderBgr(Point position)
        {
            var bgr = new SDL.SDL_Rect()
            {
                x = PADDING,
                y = position.Y - PADDING - SurfaceCache.h / 2,
                w = UxContext.ScreenSize.Width - 2 * PADDING,
                h = SurfaceCache.h + 2 * PADDING
            };
            SDL.SDL_GetRenderDrawColor(UxContext.WRenderer, out byte r, out byte g, out byte b, out byte a);
            SDL.SDL_SetRenderDrawColor(UxContext.WRenderer, 0x00, 0x00, 0x00, BGR_OPACITY);
            SDL.SDL_RenderFillRect(UxContext.WRenderer, ref bgr);
            SDL.SDL_SetRenderDrawColor(UxContext.WRenderer, r, g, b, a);
        }
    }
}
