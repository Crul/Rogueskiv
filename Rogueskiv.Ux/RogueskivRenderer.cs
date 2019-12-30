using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Ux.Renderers;
using SDL2;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Engine;
using Seedwork.Ux;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using static SDL2.SDL;

namespace Rogueskiv.Ux
{
    public class RogueskivRenderer : BufferedGameRenderer
    {
        private const int FONT_SIZE = 18;
        private readonly UxContext UxContext;
        private readonly IRogueskivUxConfig UxConfig;
        private readonly IPositionComp PlayerPositionComp;
        private readonly IntPtr Font;
        private readonly IntPtr BoardTexture;

        public RogueskivRenderer(
            UxContext uxContext,
            IGameContext gameContext,
            IRenderizable game,
            IRogueskivUxConfig uxConfig
        )
            : base(uxContext, game)
        {
            UxContext = uxContext;
            UxConfig = uxConfig;
            PlayerPositionComp = game.Entities.GetSingleComponent<PlayerComp, CurrentPositionComp>();

            Font = SDL_ttf.TTF_OpenFont(uxConfig.FontFile, FONT_SIZE);
            BoardTexture = SDL_image.IMG_LoadTexture(
                uxContext.WRenderer,
                Path.Combine("imgs", "board.png")
            );

            var bgrRenderer = new BgrRenderer(uxContext, Path.Combine("imgs", "bgr.png"), new Size(1920, 1440));
            Renderers.Add(bgrRenderer);
            Renderers.Add(new BoardRenderer(uxContext, game, BoardTexture));

            CompRenderers[typeof(FoodComp)] = new FoodRenderer(this, uxContext, game, BoardTexture);
            CompRenderers[typeof(TorchComp)] = new TorchRenderer(this, uxContext, game, BoardTexture);
            CompRenderers[typeof(MapRevealerComp)] = new MapRevealerRenderer(this, uxContext, game, BoardTexture);
            CompRenderers[typeof(AmuletComp)] = new AmuletRenderer(this, uxContext, game, BoardTexture);
            CompRenderers[typeof(EnemyComp)] = new EnemyRenderer(uxContext, game);
            CompRenderers[typeof(FOVComp)] = new FOVRenderer(uxContext);
            CompRenderers[typeof(PlayerComp)] = new PlayerRenderer(uxContext, game, uxConfig.PlayerRadius);
            CompRenderers[typeof(HealthComp)] = new HealthRenderer(uxContext);

            if (uxConfig.InGameTimeVisible || uxConfig.RealTimeVisible)
                CompRenderers[typeof(TimerComp)] = new TimerRenderer(
                    uxContext,
                    gameContext,
                    Font,
                    inGameTimeVisible: uxConfig.InGameTimeVisible,
                    realTimeVisible: uxConfig.RealTimeVisible
                );

            CompRenderers[typeof(PopUpComp)] = new PopUpRenderer(uxContext, game, Font);
        }

        public override void Reset() =>
            PlayerRenderer.SetUxCenter(UxContext, PlayerPositionComp.Position);

        protected override void RenderGame(float interpolation)
        {
            PlayerRenderer.SetUxCenter(UxContext, PlayerPositionComp.Position, UxConfig.CameraMovementFriction);
            base.RenderGame(interpolation);
        }

        public override void RecreateTextures()
        {
            base.RecreateTextures();
            var boardRenderer = (BoardRenderer)Renderers.Where(r => r is BoardRenderer).Single();
            boardRenderer.RecreateBuffer(Game, BoardTexture);
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
