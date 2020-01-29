using Rogueskiv.Menus.MenuOptions;
using Seedwork.Ux;
using static SDL2.SDL;

namespace Rogueskiv.Menus.Renderers
{
    // TODO do not render if no changes ?
    public class RogueskivMenuRenderer : GameRenderer
    {
        private const int TITLE_FONT_SIZE = 48;
        private const int MENU_FONT_SIZE = 20;
        private const int SMALL_FONT_SIZE = 14;

        public RogueskivMenuRenderer(UxContext uxContext, RogueskivMenu game, string fontFile)
            : base(uxContext, game)
        {
            var titleFont = uxContext.GetFont(fontFile, TITLE_FONT_SIZE);
            var menuFont = uxContext.GetFont(fontFile, MENU_FONT_SIZE);
            var smallFont = uxContext.GetFont(fontFile, SMALL_FONT_SIZE);

            Renderers.Add(new TitleRenderer(uxContext, titleFont, smallFont));
            Renderers.Add(new InstructionsRenderer(uxContext, smallFont));
            Renderers.Add(new ControlsInfoRenderer(uxContext, game, smallFont));
            Renderers.Add(new CustomSeedInputRenderer(uxContext, game, smallFont));
            CompRenderers[typeof(MenuOptionComp)] = new MenuOptionRenderer(uxContext, game, menuFont);
            CompRenderers[typeof(StatsComp)] = new StatsRenderer(uxContext, smallFont);
        }

        protected override void RenderGame(float interpolation)
        {
            SDL_RenderClear(WRenderer);
            base.RenderGame(interpolation);
        }
    }
}
