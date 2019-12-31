using Rogueskiv.Core;
using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.GameEvents;
using SDL2;
using Seedwork.Core.Entities;
using Seedwork.Crosscutting;
using System.Linq;

namespace Rogueskiv.Ux.EffectPlayers
{
    class PlayerMovementEffectPlayer : EffectPlayer
    {
        private const float VOLUME_CHANGE_RATE = 0.9f;

        private readonly RogueskivGame Game;
        private readonly BoundedMovementComp PlayerMovementComp;
        private readonly CurrentPositionComp PlayerPositionComp;
        private readonly LastPositionComp PlayerLastPositionComp;

        private int LastVolume = 0;

        public PlayerMovementEffectPlayer(RogueskivGame game, int channel = -1)
            : base(audioFilename: "player_movement", channel)
        {
            Game = game;
            PlayerMovementComp = game.Entities.GetSingleComponent<PlayerComp, BoundedMovementComp>();
            PlayerPositionComp = game.Entities.GetSingleComponent<PlayerComp, CurrentPositionComp>();
            PlayerLastPositionComp = game.Entities.GetSingleComponent<PlayerComp, LastPositionComp>();
        }

        public override void Play()
        {
            var volume = 0;
            var wentThroughStairs = Game.GameEvents.Any(ev => ev is IStairsEvent);
            if (!wentThroughStairs)
            {
                var lastMovementDistance = Distance.Get(
                    PlayerLastPositionComp.Position.Substract(PlayerPositionComp.Position)
                );
                var speedFactor = lastMovementDistance / PlayerMovementComp.MaxSpeed;
                var volumeTarget = (speedFactor * SDL_mixer.MIX_MAX_VOLUME * 0.6f);
                volume = (int)(LastVolume + (volumeTarget - LastVolume) * VOLUME_CHANGE_RATE);
            }

            if (volume == 0)
                PauseChunk();
            else
            {
                if (IsPlaying())
                    SetVolume(volume);
                else
                    PlayChunk(volume);
            }

            // TODO stop chunk between levels... or reuse EfectPlayers ?

            LastVolume = volume;
        }
    }
}
