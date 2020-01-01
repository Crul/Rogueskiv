using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;

namespace Rogueskiv.Ux.EffectPlayers
{
    class EnemyCollidedEffectPlayer : EventEffectPlayer<EnemyCollidedEvent>
    {
        public EnemyCollidedEffectPlayer(RogueskivGame game)
            : base(game, "enemy_collided")
        { }
    }
}
