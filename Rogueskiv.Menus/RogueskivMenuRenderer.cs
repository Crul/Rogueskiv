using Rogueskiv.Menus.MenuOptions;
using SDL2;
using Seedwork.Ux;
using System;
using static SDL2.SDL;

namespace Rogueskiv.Menus
{
    // TODO do not render if no changes ?
    public class RogueskivMenuRenderer : GameRenderer
    {
        private const int TITLE_FONT_SIZE = 48;
        private const int MENU_FONT_SIZE = 24;
        private const int SMALL_FONT_SIZE = 14;
        private readonly IntPtr TitleFont;
        private readonly IntPtr MenuFont;
        private readonly IntPtr SmallFont;

        public RogueskivMenuRenderer(UxContext uxContext, RogueskivMenu game, string fontFile)
            : base(uxContext, game)
        {
            TitleFont = SDL_ttf.TTF_OpenFont(fontFile, TITLE_FONT_SIZE);
            MenuFont = SDL_ttf.TTF_OpenFont(fontFile, MENU_FONT_SIZE);
            SmallFont = SDL_ttf.TTF_OpenFont(fontFile, SMALL_FONT_SIZE);

            Renderers.Add(new TitleRenderer(uxContext, TitleFont));
            Renderers.Add(new InstructionsRenderer(uxContext, SmallFont));
            Renderers.Add(new CustomSeedInputRenderer(uxContext, game, MenuFont));
            CompRenderers[typeof(MenuOptionComp)] = new MenuOptionRenderer(uxContext, game, MenuFont);
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
                SDL_ttf.TTF_CloseFont(SmallFont);
            }
        }
    }
}
