using Rogueskiv.Core;
using Rogueskiv.Core.Components;
using Seedwork.Core.Entities;
using Seedwork.Crosscutting;
using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using System;
using System.Drawing;
using System.Linq;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class PopUpRenderer : TextCompRenderer<PopUpComp>
    {
        private const int PADDING = 60;
        private const byte BGR_OPACITY = 0xF0;
        private const int LINE_HEIGHT = 24;
        private readonly RogueskivGame Game;

        public PopUpRenderer(UxContext uxContext, RogueskivGame game, IntPtr font)
            : base(uxContext, font) => Game = game;

        protected override void Render(IEntity entity, PopUpComp popUpComp, float interpolation)
        {
            if (!Game.Pause)
                return;

            var textLines = GetText(popUpComp).Split(Environment.NewLine).ToList();
            var position = GetPosition(popUpComp);
            position.Y -= textLines.Count * LINE_HEIGHT / 2;
            var textColor = GetColor(popUpComp);
            var aligment = GetAligment();

            TextRenderer.Render(
                textLines.First(),
                textColor,
                position,
                aligment,
                position => RenderBgr(position, textLines.Count)
            );

            textLines.Skip(1).ToList().ForEach(textLine =>
            {
                position = position.Add(y: LINE_HEIGHT);
                if (!string.IsNullOrEmpty(textLine))
                    TextRenderer.Render(textLine, textColor, position, aligment);
            });
        }

        protected override SDL_Color GetColor(PopUpComp component) =>
            new SDL_Color() { r = 0xFF, g = 0xFF, b = 0xFF, a = 0xFF };

        protected override Point GetPosition(PopUpComp component) =>
            UxContext.ScreenSize.Divide(2).ToPoint();

        protected override string GetText(PopUpComp component)
        {
            var resultCode = Game.Result?.ResultCode;

            if (resultCode == RogueskivGameResults.DeathResult.ResultCode)
                return $"YOU'RE DEAD"
                    + $"{Environment.NewLine}Press Q to go to the menu."
                    + $"{Environment.NewLine}"
                    + $"{Environment.NewLine}Seed: {Game.GameSeed}";

            if (resultCode == RogueskivGameResults.WinResult.ResultCode)
                return $"YOU WIN!"
                    + $"{Environment.NewLine}"
                    + $"{Environment.NewLine}In Game Time: {Game.GameStats.GetInGameTimeFormatted()}"
                    + $"{Environment.NewLine}Real Time: {Game.GameStats.GetRealTimeFormatted()}"
                    + $"{Environment.NewLine}"
                    + $"{Environment.NewLine}Press Q to go to the menu or ESC to continue playing."
                    + $"{Environment.NewLine}"
                    + $"{Environment.NewLine}Seed: {Game.GameSeed}";

            return component.Text;
        }

        protected override void RenderBgr(Point position) =>
            throw new NotImplementedException();

        private void RenderBgr(Point position, int textLines)
        {
            var bgr = new SDL_Rect()
            {
                x = PADDING,
                y = position.Y - PADDING,
                w = UxContext.ScreenSize.Width - 2 * PADDING,
                h = TextRenderer.SurfaceCache.h * textLines + 2 * PADDING
            };
            SDL_GetRenderDrawColor(UxContext.WRenderer, out byte r, out byte g, out byte b, out byte a);
            SDL_SetRenderDrawColor(UxContext.WRenderer, 0x00, 0x00, 0x00, BGR_OPACITY);
            SDL_RenderFillRect(UxContext.WRenderer, ref bgr);
            SDL_SetRenderDrawColor(UxContext.WRenderer, r, g, b, a);
        }
    }
}
