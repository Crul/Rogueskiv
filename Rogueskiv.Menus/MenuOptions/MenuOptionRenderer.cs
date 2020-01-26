using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using System;
using System.Drawing;
using static SDL2.SDL;

namespace Rogueskiv.Menus.MenuOptions
{
    class MenuOptionRenderer : TextCompRenderer<MenuOptionComp>
    {
        private const int MARGIN_LEFT = 100;
        private const int MARGIN_TOP = 136;
        private const int LINE_HEIGHT = 45;

        private readonly RogueskivMenu Game;

        public MenuOptionRenderer(UxContext uxContext, RogueskivMenu game, IntPtr font)
            : base(uxContext, font) =>
            Game = game;

        protected override string GetText(MenuOptionComp component) => component.GetText();

        protected override SDL_Color GetColor(MenuOptionComp component) =>
            component.Active
                ? (Game.AskingForCustomSeed
                    ? new SDL_Color { r = 0xCC, g = 0xCC, b = 0xCC, a = 0xFF }
                    : new SDL_Color { r = 0xFF, g = 0xFF, b = 0xFF, a = 0xFF }
                )
                : (Game.AskingForCustomSeed
                    ? new SDL_Color { r = 0x44, g = 0x44, b = 0x44, a = 0xFF }
                    : new SDL_Color { r = 0x99, g = 0x99, b = 0x99, a = 0xFF }
                );

        protected override Point GetPosition(MenuOptionComp component) =>
            new Point(MARGIN_LEFT, MARGIN_TOP + (component.Order * LINE_HEIGHT));

        protected override TextAlign GetAligment() => TextAlign.TOP_LEFT;
    }
}
