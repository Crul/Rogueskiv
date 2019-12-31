using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;

namespace Rogueskiv.Ux.EffectPlayers
{
    class DeathEffectPlayer : EventEffectPlayer<DeathEvent>
    {
        public DeathEffectPlayer(RogueskivGame game, int channel = -1)
            : base(game, "death", channel)
        { }
    }
}
