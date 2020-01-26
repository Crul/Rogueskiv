using Seedwork.Crosscutting;
using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using System;
using System.Collections.Generic;
using System.Drawing;
using static SDL2.SDL;

namespace Rogueskiv.Menus
{
    class ControlsInfoRenderer : IRenderer
    {
        private const int MIN_MARGIN_X_LEFT = 390;
        private const int MARGIN_X_RIGHT = 250;
        private const int MARGIN_Y = 144;
        private const int LINE_HEIGHT = 24;
        private readonly List<string> InstructionsTextLines = new List<string>
        {
            "Game controls:",
            "- Cursors to move",
            "- ESC to pause or exit",
            "- S to toggle sound",
            "- M to toggle music",
        };

        private readonly SDL_Color InstructionsColor =
            new SDL_Color() { r = 0xDD, g = 0xDD, b = 0xDD, a = 0xFF };

        protected readonly UxContext UxContext;
        protected readonly RogueskivMenu Game;
        protected readonly TextRenderer TextRenderer;

        public ControlsInfoRenderer(UxContext uxContext, RogueskivMenu game, IntPtr font)
        {
            UxContext = uxContext;
            Game = game;
            TextRenderer = new TextRenderer(uxContext, font);
        }

        public void Render()
        {
            if (Game.AskingForCustomSeed)
                return;

            var position = new Point(x: GetX(), y: GetY());
            InstructionsTextLines.ForEach(textLine =>
            {
                TextRenderer.Render(textLine, InstructionsColor, position, TextAlign.TOP_LEFT);
                position = position.Add(y: LINE_HEIGHT);
            });
        }

        private int GetX() => Math.Max(MIN_MARGIN_X_LEFT, UxContext.ScreenSize.Width - MARGIN_X_RIGHT);

        private int GetY() => MARGIN_Y;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool cleanManagedResources)
        {
            if (cleanManagedResources)
                TextRenderer.Dispose();
        }
    }
}
