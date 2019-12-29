using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Ux.Renderers;
using SDL2;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Ux;
using System;
using System.Drawing;
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
#pragma warning disable IDE0069 // Disposable fields should be disposed
#pragma warning disable CA2213 // Disposable fields should be disposed
        private readonly BoardRenderer BoardRenderer;
#pragma warning restore CA2213 // Disposable fields should be disposed
#pragma warning restore IDE0069 // Disposable fields should be disposed

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

            var bgrRenderer = new BgrRenderer(uxContext, Path.Combine("imgs", "bgr.png"), new Size(1920, 1440));
            Renderers.Add(bgrRenderer);

            BoardRenderer = new BoardRenderer(uxContext, game, BoardTexture);
            CompRenderers[typeof(BoardComp)] = BoardRenderer;
            CompRenderers[typeof(FoodComp)] = new FoodRenderer(this, uxContext, game, BoardTexture);
            CompRenderers[typeof(TorchComp)] = new TorchRenderer(this, uxContext, game, BoardTexture);
            CompRenderers[typeof(MapRevealerComp)] = new MapRevealerRenderer(this, uxContext, game, BoardTexture);
            CompRenderers[typeof(AmuletComp)] = new AmuletRenderer(this, uxContext, game, BoardTexture);
            CompRenderers[typeof(EnemyComp)] = new EnemyRenderer(uxContext, game);
            CompRenderers[typeof(FOVComp)] = new FOVRenderer(uxContext);
            CompRenderers[typeof(PlayerComp)] = new PlayerRenderer(uxContext, game);
            CompRenderers[typeof(HealthComp)] = new HealthRenderer(uxContext);
            CompRenderers[typeof(PopUpComp)] = new PopUpRenderer(uxContext, game, Font);
        }

        public override void Reset() =>
            PlayerRenderer.SetUxCenter(UxContext, PlayerPositionComp.Position);

        protected override void RenderGame(float interpolation)
        {
            PlayerRenderer.SetUxCenter(UxContext, PlayerPositionComp.Position, PlayerRenderer.CAMERA_MOVEMENT_FRICTION);
            base.RenderGame(interpolation);
        }

        public override void RecreateTextures()
        {
            base.RecreateTextures();
            BoardRenderer.RecreateBuffer(Game, BoardTexture);
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
