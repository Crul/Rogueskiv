using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;

namespace Rogueskiv.Ux.EffectPlayers
{
    class DeathEffectPlayer : EventEffectPlayer<DeathEvent>
    {
        public DeathEffectPlayer(RogueskivGame game) : base(game, "death") { }
    }
}
