using Rogueskiv.Menus.MenuOptions;
using Seedwork.Core.Entities;
using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using System;
using System.Drawing;
using static SDL2.SDL;

namespace Rogueskiv.Menus.Renderers
{
    class MenuOptionRenderer : TextCompRenderer<MenuOptionComp>
    {
        private const int MIN_MARGIN_X = 40;
        private const int MAX_MARGIN_DELTA_X = 100 - MIN_MARGIN_X;
        private const float MIN_SCREEN_WIDTH_TO_ADJUST_MARGIN = 720;
        private const float MAX_SCREEN_WIDTH_TO_ADJUST_MARGIN = 1200;
        private const int MARGIN_Y = 106;
        private const int LINE_HEIGHT = 32;

        private readonly RogueskivMenu Game;

        public MenuOptionRenderer(UxContext uxContext, RogueskivMenu game, IntPtr font)
            : base(uxContext, font) =>
            Game = game;

        protected override void Render(IEntity entity, MenuOptionComp comp, float interpolation)
        {
            if (Game.IsMainMenuView)
                base.Render(entity, comp, interpolation);
        }

        protected override string GetText(MenuOptionComp component) => component.GetText();

        protected override SDL_Color GetColor(MenuOptionComp component) =>
            component.Active
                ? (Game.IsCustomSeedInput
                    ? new SDL_Color { r = 0xCC, g = 0xCC, b = 0xCC, a = 0xFF }
                    : new SDL_Color { r = 0xFF, g = 0xFF, b = 0xFF, a = 0xFF }
                )
                : (Game.IsCustomSeedInput
                    ? new SDL_Color { r = 0x44, g = 0x44, b = 0x44, a = 0xFF }
                    : new SDL_Color { r = 0x99, g = 0x99, b = 0x99, a = 0xFF }
                );

        protected override Point GetPosition(MenuOptionComp component) =>
            new Point(GetX(), GetY(component));

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

        private int GetY(MenuOptionComp component)
            => MARGIN_Y + (component.Order * LINE_HEIGHT);

        protected override TextAlign GetAligment() => TextAlign.TOP_LEFT;
    }
}
