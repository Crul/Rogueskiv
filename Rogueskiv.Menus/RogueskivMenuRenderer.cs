﻿using Rogueskiv.Menus.MenuOptions;
using SDL2;
using Seedwork.Core;
using Seedwork.Ux;
using System;
using System.IO;

namespace Rogueskiv.Menus
{
    // TODO do not render if no changes ?
    public class RogueskivMenuRenderer : GameRenderer
    {
        private const int FONT_SIZE = 28;
        private readonly IntPtr MenuFont;

        public RogueskivMenuRenderer(UxContext uxContext, IRenderizable game)
            : base(uxContext, game)
        {
            MenuFont = SDL_ttf.TTF_OpenFont(Path.Join("fonts", "Hack", "Hack-Regular.ttf"), FONT_SIZE);
            Renderers[typeof(MenuOptionComp)] = new MenuOptionRenderer(uxContext, MenuFont);
        }

        protected override void Dispose(bool cleanManagedResources)
        {
            base.Dispose(cleanManagedResources);
            SDL_ttf.TTF_CloseFont(MenuFont);
        }
    }
}
