﻿using Seedwork.Crosscutting;
using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using System;
using System.Collections.Generic;
using System.Drawing;
using static SDL2.SDL;

namespace Rogueskiv.Menus
{
    class InstructionsRenderer : IRenderer
    {
        private const int MIN_MARGIN_X = 20;
        private const int MAX_MARGIN_DELTA_X = 140 + MIN_MARGIN_X;
        private const int MIN_Y = 320;
        private const int MARGIN_Y = 60;
        private const int LINE_HEIGHT = 24;
        private const float MIN_SCREEN_WIDTH_TO_ADJUST_MARGIN = 1000;
        private const float MAX_SCREEN_WIDTH_TO_ADJUST_MARGIN = 1200;
        private readonly List<string> InstructionsTextLines = new List<string>
        {
            "Only keyboard supported (no mouse)",
            "Use cursors and press ENTER to select an option.",
            "While playing, use the cursors to move and ESC key to pause and Q to exit).",
            "WARNING! Work in progress: This is a proof of concept, not a fully developed game."
        };

        private readonly SDL_Color InstructionsColor =
            new SDL_Color() { r = 0xDD, g = 0xDD, b = 0xDD, a = 0xFF };

        protected readonly UxContext UxContext;
        protected readonly TextRenderer TextRenderer;

        public InstructionsRenderer(UxContext uxContext, IntPtr font)
        {
            UxContext = uxContext;
            TextRenderer = new TextRenderer(uxContext, font);
        }

        public void Render()
        {
            var position = new Point(x: GetX(), y: GetY());
            InstructionsTextLines.ForEach(textLine =>
            {
                TextRenderer.Render(textLine, InstructionsColor, position, TextAlign.BOTTOM_LEFT);
                position = position.Add(y: LINE_HEIGHT);
            });
        }

        private int GetX() =>
            MIN_MARGIN_X
            + (int)(
                MAX_MARGIN_DELTA_X
                * (UxContext.ScreenSize.Width > MAX_SCREEN_WIDTH_TO_ADJUST_MARGIN
                    ? 1f
                    : Math.Max(
                        0f,
                        (UxContext.ScreenSize.Width - MIN_SCREEN_WIDTH_TO_ADJUST_MARGIN)
                            / (MAX_SCREEN_WIDTH_TO_ADJUST_MARGIN - MIN_SCREEN_WIDTH_TO_ADJUST_MARGIN)
                    )
                )
            );

        private int GetY() =>
            Math.Max(
                MIN_Y,
                UxContext.ScreenSize.Height - MARGIN_Y - (InstructionsTextLines.Count * LINE_HEIGHT)
            );

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
