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
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class PopUpRenderer : TextCompRenderer<PopUpComp>
    {
        private const int PADDING = 20;
        private const byte BGR_OPACITY = 0xF0;
        private readonly IRenderizable Game;

        public PopUpRenderer(UxContext uxContext, IRenderizable game, IntPtr font)
            : base(uxContext, font) => Game = game;

        protected override void Render(IEntity entity, PopUpComp popUpComp, float interpolation)
        {
            if (!Game.Pause)
                return;

            base.Render(entity, popUpComp, interpolation);
        }

        protected override SDL.SDL_Color GetColor(PopUpComp component) =>
            new SDL_Color() { r = 0xFF, g = 0xFF, b = 0xFF, a = 0xFF };

        protected override Point GetPosition(PopUpComp component) =>
            UxContext.ScreenSize.Divide(2).ToPoint();

        protected override string GetText(PopUpComp component)
        {
            var resultCode = Game.Result?.ResultCode;

            if (resultCode == RogueskivGameResults.DeathResult.ResultCode)
                return "YOU'RE DEAD";

            if (resultCode == RogueskivGameResults.WinResult.ResultCode)
                return "YOU WIN";

            return component.Text;
        }

        protected override void RenderBgr(Point position)
        {
            var bgr = new SDL_Rect()
            {
                x = PADDING,
                y = position.Y - PADDING - TextRenderer.SurfaceCache.h / 2,
                w = UxContext.ScreenSize.Width - 2 * PADDING,
                h = TextRenderer.SurfaceCache.h + 2 * PADDING
            };
            SDL_GetRenderDrawColor(UxContext.WRenderer, out byte r, out byte g, out byte b, out byte a);
            SDL_SetRenderDrawColor(UxContext.WRenderer, 0x00, 0x00, 0x00, BGR_OPACITY);
            SDL_RenderFillRect(UxContext.WRenderer, ref bgr);
            SDL_SetRenderDrawColor(UxContext.WRenderer, r, g, b, a);
        }
    }
}
