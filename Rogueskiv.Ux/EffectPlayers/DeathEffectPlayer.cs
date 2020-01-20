using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;
using Seedwork.Ux;

namespace Rogueskiv.Ux.EffectPlayers
{
    class DeathEffectPlayer : EventEffectPlayer<DeathEvent>
    {
        public DeathEffectPlayer(UxContext uxContext, RogueskivGame game)
            : base(uxContext, game, "death") { }
    }
}
