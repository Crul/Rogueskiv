using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;
using Seedwork.Ux;

namespace Rogueskiv.Ux.EffectPlayers
{
    class EnemyCollidedEffectPlayer : EventEffectPlayer<EnemyCollidedEvent>
    {
        public EnemyCollidedEffectPlayer(UxContext uxContext, RogueskivGame game)
            : base(uxContext, game, "enemy_collided") { }
    }
}
