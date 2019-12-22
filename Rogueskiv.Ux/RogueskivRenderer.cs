using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Walls;
using Rogueskiv.Ux.Renderers;
using SDL2;
using Seedwork.Core;
using Seedwork.Ux;
using System;
using System.IO;
using static SDL2.SDL;

namespace Rogueskiv.Ux
{
    public class RogueskivRenderer : GameRenderer
    {
        private const int FONT_SIZE = 28;
        private readonly IntPtr Font;
        private readonly IntPtr BoardTexture;

        public RogueskivRenderer(UxContext uxContext, IRenderizable game, string fontFile)
            : base(uxContext, game)
        {
            Font = SDL_ttf.TTF_OpenFont(fontFile, FONT_SIZE);

            BoardTexture = SDL_image.IMG_LoadTexture(
                uxContext.WRenderer,
                Path.Combine("imgs", "board.png")
            );

            Renderers[typeof(TileComp)] = new TileRenderer(uxContext, BoardTexture);
            Renderers[typeof(IWallComp)] = new WallRenderer(uxContext, game, BoardTexture);
            Renderers[typeof(DownStairsComp)] = new DownStairsRenderer(uxContext, BoardTexture);
            Renderers[typeof(UpStairsComp)] = new UpStairsRenderer(uxContext, BoardTexture);
            Renderers[typeof(EnemyComp)] = new EnemyRenderer(uxContext);
            Renderers[typeof(FOVComp)] = new FOVRenderer(uxContext, game);
            Renderers[typeof(PlayerComp)] = new PlayerRenderer(uxContext);
            Renderers[typeof(HealthComp)] = new HealthRenderer(uxContext);
            Renderers[typeof(PopUpComp)] = new PopUpRenderer(uxContext, game, Font);
        }

        protected override void Dispose(bool cleanManagedResources)
        {
            base.Dispose(cleanManagedResources);
            SDL_ttf.TTF_CloseFont(Font);
            SDL_DestroyTexture(BoardTexture);
        }
    }
}
