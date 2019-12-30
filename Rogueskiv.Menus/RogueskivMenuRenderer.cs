using Rogueskiv.Menus.MenuOptions;
using SDL2;
using Seedwork.Core;
using Seedwork.Ux;
using System;
using static SDL2.SDL;

namespace Rogueskiv.Menus
{
    // TODO do not render if no changes ?
    public class RogueskivMenuRenderer : GameRenderer
    {
        private const int TITLE_FONT_SIZE = 48;
        private const int MENU_FONT_SIZE = 30;
        private readonly IntPtr TitleFont;
        private readonly IntPtr MenuFont;

        public RogueskivMenuRenderer(UxContext uxContext, IRenderizable game, string fontFile)
            : base(uxContext, game)
        {
            TitleFont = SDL_ttf.TTF_OpenFont(fontFile, TITLE_FONT_SIZE);
            Renderers.Add(new TitleRenderer(uxContext, TitleFont));
            MenuFont = SDL_ttf.TTF_OpenFont(fontFile, MENU_FONT_SIZE);
            CompRenderers[typeof(MenuOptionComp)] = new MenuOptionRenderer(uxContext, MenuFont);
        }

        protected override void RenderGame(float interpolation)
        {
            SDL_RenderClear(WRenderer);
            base.RenderGame(interpolation);
        }

        protected override void Dispose(bool cleanManagedResources)
        {
            base.Dispose(cleanManagedResources);
            if (cleanManagedResources)
            {
                SDL_ttf.TTF_CloseFont(TitleFont);
                SDL_ttf.TTF_CloseFont(MenuFont);
            }
        }
    }
}
