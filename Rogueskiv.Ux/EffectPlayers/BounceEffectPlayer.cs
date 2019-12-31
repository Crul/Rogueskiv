using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;
using System;

namespace Rogueskiv.Ux.EffectPlayers
{
    class BounceEffectPlayer : EventEffectPlayer<PlayerHitWallEvent>
    {
        public BounceEffectPlayer(RogueskivGame game) : base(game, "rock_hit") { }

        protected override int GetVolume(PlayerHitWallEvent playerHitWallEvent)
        {
            var speedFactor = Math.Min(playerHitWallEvent.SpeedFactor, 1);
            var volume = (int)(0.5f * Math.Pow(speedFactor, 2) * base.GetVolume(playerHitWallEvent));

            return volume;
        }
    }
}
