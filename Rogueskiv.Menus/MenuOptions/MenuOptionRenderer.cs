﻿using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using System;
using static SDL2.SDL;

namespace Rogueskiv.Menus.MenuOptions
{
    class MenuOptionRenderer : TextRenderer<MenuOptionComp>
    {
        public MenuOptionRenderer(UxContext uxContext, IntPtr font)
            : base(uxContext, font)
        { }

        protected override string GetText(MenuOptionComp component) => component.Text;

        protected override SDL_Color GetColor(MenuOptionComp component) =>
            component.Active
                ? new SDL_Color { r = 0xFF, g = 0xFF, b = 0xFF, a = 0xFF }
                : new SDL_Color { r = 0x99, g = 0x99, b = 0x99, a = 0xFF };

        protected override (int x, int y) GetPosition(MenuOptionComp component) =>
            (100, 100 + (component.Order * 50));
    }
}