using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;

namespace Rogueskiv.Ux.EffectPlayers
{
    class DeathEffectPlayer : EffectPlayer<DeathEvent>
    {
        public DeathEffectPlayer(RogueskivGame game, int channel = -1)
            : base(game, "death", channel)
        { }

        protected override int GetVolume(DeathEvent gameEvent) =>
            base.GetVolume(gameEvent) / 10;
    }
}
