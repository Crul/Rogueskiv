using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using System;
using System.Drawing;
using static SDL2.SDL;

namespace Rogueskiv.Menus.MenuOptions
{
    class CustomSeedInputRenderer : IRenderer
    {
        private readonly RogueskivMenu Game;
        private readonly TextRenderer TextRenderer;
        private readonly SDL_Color TextColor = new SDL_Color() { r = 0xFF, g = 0xFF, b = 0xFF };
        private readonly Point InputTextPosition = new Point(400, 186);

        public CustomSeedInputRenderer(UxContext uxContext, RogueskivMenu game, IntPtr font)
        {
            Game = game;
            TextRenderer = new TextRenderer(uxContext, font);
        }

        public void Render()
        {
            if (Game.IsCustomSeedInput)
                TextRenderer.Render(
                    $"Enter seed: {Game.CustomSeedText}_",
                    TextColor,
                    InputTextPosition,
                    TextAlign.TOP_LEFT
                );
        }

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
