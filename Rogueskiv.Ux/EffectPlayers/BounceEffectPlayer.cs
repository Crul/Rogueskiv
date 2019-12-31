using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;
using System;

namespace Rogueskiv.Ux.EffectPlayers
{
    class BounceEffectPlayer : EventEffectPlayer<PlayerHitWallEvent>
    {
        public BounceEffectPlayer(RogueskivGame game, int channel = -1)
            : base(game, "rock_hit", channel)
        { }

        protected override int GetVolume(PlayerHitWallEvent playerHitWallEvent)
        {
            var speedFactor = Math.Min(playerHitWallEvent.SpeedFactor, 1);
            var volume = (int)(0.1f * Math.Pow(speedFactor, 3) * base.GetVolume(playerHitWallEvent));

            return volume;
        }
    }
}
