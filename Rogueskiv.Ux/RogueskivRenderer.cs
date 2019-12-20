using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Ux.Renderers;
using SDL2;
using Seedwork.Core;
using Seedwork.Ux;
using System;

namespace Rogueskiv.Ux
{
    public class RogueskivRenderer : GameRenderer
    {
        private const int FONT_SIZE = 28;
        private readonly IntPtr Font;

        public RogueskivRenderer(UxContext uxContext, IRenderizable game, string fontFile)
            : base(uxContext, game)
        {
            Font = SDL_ttf.TTF_OpenFont(fontFile, FONT_SIZE);

            Renderers[typeof(TileComp)] = new TileRenderer(uxContext);
            Renderers[typeof(DownStairsComp)] = new DownStairsRenderer(uxContext);
            Renderers[typeof(UpStairsComp)] = new UpStairsRenderer(uxContext);
            Renderers[typeof(EnemyComp)] = new EnemyRenderer(uxContext);
            Renderers[typeof(IFOVComp)] = new FOVRenderer(uxContext);
            Renderers[typeof(PlayerComp)] = new PlayerRenderer(uxContext);
            Renderers[typeof(PopUpComp)] = new PopUpRenderer(uxContext, game, Font);
        }

        protected override void Dispose(bool cleanManagedResources)
        {
            base.Dispose(cleanManagedResources);
            SDL_ttf.TTF_CloseFont(Font);
        }
    }
}
