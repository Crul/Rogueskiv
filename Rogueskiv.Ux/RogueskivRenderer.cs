using Rogueskiv.Core;
using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.GameEvents;
using Rogueskiv.Ux.EffectPlayers;
using Rogueskiv.Ux.Renderers;
using SDL2;
using Seedwork.Core.Entities;
using Seedwork.Engine;
using Seedwork.Ux;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Rogueskiv.Ux
{
    public class RogueskivRenderer : BufferedGameRenderer
    {
        private const int FONT_SIZE = 18;
        private readonly UxContext UxContext;
        private readonly RogueskivGame RogueskivGame;
        private readonly IRogueskivUxConfig UxConfig;
        private readonly IPositionComp PlayerPositionComp;
        private readonly IntPtr Font;
        private readonly IntPtr BoardTexture;
        private readonly PlayerMovementEffectPlayer PlayerMovementEffectPlayer;

        private readonly List<IEffectPlayer> EffectPlayers = new List<IEffectPlayer>();

        public RogueskivRenderer(
            UxContext uxContext,
            IGameContext gameContext,
            RogueskivGame game,
            IRogueskivUxConfig uxConfig
        ) : base(uxContext, game)
        {
            RogueskivGame = game;
            UxContext = uxContext;
            UxConfig = uxConfig;
            PlayerPositionComp = game.Entities.GetSingleComponent<PlayerComp, CurrentPositionComp>();

            Font = SDL_ttf.TTF_OpenFont(uxConfig.FontFile, FONT_SIZE);
            BoardTexture = uxContext.GetTexture("board.png");

            var bgrRenderer = new BgrRenderer(uxContext, new Size(1920, 1440));
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
            CompRenderers[typeof(TimerComp)] = new GameInfoRenderer(
                uxContext,
                gameContext,
                Font,
                game.Floor,
                inGameTimeVisible: uxConfig.InGameTimeVisible,
                realTimeVisible: uxConfig.RealTimeVisible
            );
            CompRenderers[typeof(PopUpComp)] = new PopUpRenderer(uxContext, game, Font);

            PlayerMovementEffectPlayer = new PlayerMovementEffectPlayer(uxContext, game);
            EffectPlayers.Add(new BounceEffectPlayer(uxContext, game));
            EffectPlayers.Add(new TorchPickedEffectPlayer(uxContext, game));
            EffectPlayers.Add(new MapRevealerPickedEffectPlayer(uxContext, game));
            EffectPlayers.Add(new FoodPickedEffectPlayer(uxContext, game));
            EffectPlayers.Add(new WinEffectPlayer(uxContext, game));
            EffectPlayers.Add(new EnemyCollidedEffectPlayer(uxContext, game));
            EffectPlayers.Add(new StairsUpEffectPlayer(uxContext, game));
            EffectPlayers.Add(new StairsDownEffectPlayer(uxContext, game));
            EffectPlayers.Add(new DeathEffectPlayer(uxContext, game));
        }

        public override void Stop() => PlayerMovementEffectPlayer.Stop();

        public override void Reset() =>
            PlayerRenderer.SetUxCenter(UxContext, PlayerPositionComp.Position);

        protected override void RenderGame(float interpolation)
        {
            if (RogueskivGame.GameEvents.Any(ev => ev is ToggleSoundEvent))
            {
                UxConfig.SoundsOn = !UxConfig.SoundsOn;
                if (!UxConfig.SoundsOn)
                    SDL_mixer.Mix_HaltChannel(-1);
            }

            PlayerRenderer.SetUxCenter(UxContext, PlayerPositionComp.Position, UxConfig.CameraMovementFriction);
            base.RenderGame(interpolation);
            if (UxConfig.SoundsOn)
            {
                PlayerMovementEffectPlayer.Play();
                EffectPlayers.ForEach(ep => ep.Play());
            }
            RogueskivGame.GameEvents.Clear();
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
                EffectPlayers.ForEach(ep => ep.Dispose());
                PlayerMovementEffectPlayer.Dispose();
            }
        }
    }
}
