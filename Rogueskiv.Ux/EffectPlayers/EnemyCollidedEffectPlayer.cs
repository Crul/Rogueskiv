using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;

namespace Rogueskiv.Ux.EffectPlayers
{
    class EnemyCollidedEffectPlayer : EventEffectPlayer<EnemyCollidedEvent>
    {
        public EnemyCollidedEffectPlayer(RogueskivGame game, int channel = -1)
            : base(game, "enemy_collided", channel)
        { }
    }
}
