using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Ux.Renderers;
using SDL2;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Ux;
using System;
using System.IO;
using static SDL2.SDL;

namespace Rogueskiv.Ux
{
    public class RogueskivRenderer : BufferedGameRenderer
    {
        private const int FONT_SIZE = 18;
        private readonly UxContext UxContext;
        private readonly IPositionComp PlayerPositionComp;
        private readonly IntPtr Font;
        private readonly IntPtr BoardTexture;

        public RogueskivRenderer(UxContext uxContext, IRenderizable game, string fontFile)
            : base(uxContext, game)
        {
            UxContext = uxContext;
            PlayerPositionComp = game.Entities.GetSingleComponent<PlayerComp, CurrentPositionComp>();

            Font = SDL_ttf.TTF_OpenFont(fontFile, FONT_SIZE);
            BoardTexture = SDL_image.IMG_LoadTexture(
                uxContext.WRenderer,
                Path.Combine("imgs", "board.png")
            );

            Renderers[typeof(TileComp)] = new TileRenderer(uxContext, BoardTexture);
            Renderers[typeof(DownStairsComp)] = new DownStairsRenderer(uxContext, BoardTexture);
            Renderers[typeof(UpStairsComp)] = new UpStairsRenderer(uxContext, BoardTexture);
            Renderers[typeof(FoodComp)] = new FoodRenderer(this, uxContext, BoardTexture);
            Renderers[typeof(TorchComp)] = new TorchRenderer(this, uxContext, BoardTexture);
            Renderers[typeof(MapComp)] = new MapRenderer(this, uxContext, BoardTexture);
            Renderers[typeof(AmuletComp)] = new AmuletRenderer(this, uxContext, BoardTexture);
            Renderers[typeof(EnemyComp)] = new EnemyRenderer(uxContext);
            Renderers[typeof(FOVComp)] = new FOVRenderer(uxContext, game);
            Renderers[typeof(PlayerComp)] = new PlayerRenderer(uxContext);
            Renderers[typeof(HealthComp)] = new HealthRenderer(uxContext);
            Renderers[typeof(PopUpComp)] = new PopUpRenderer(uxContext, game, Font);
        }

        public override void Reset() =>
            PlayerRenderer.SetUxCenter(UxContext, PlayerPositionComp.Position);

        protected override void RenderGame(float interpolation)
        {
            PlayerRenderer.SetUxCenter(UxContext, PlayerPositionComp.Position, PlayerRenderer.CAMERA_MOVEMENT_FRICTION);
            base.RenderGame(interpolation);
        }

        protected override void Dispose(bool cleanManagedResources)
        {
            base.Dispose(cleanManagedResources);
            if (cleanManagedResources)
            {
                SDL_ttf.TTF_CloseFont(Font);
                SDL_DestroyTexture(BoardTexture);
            }
        }
    }
}
