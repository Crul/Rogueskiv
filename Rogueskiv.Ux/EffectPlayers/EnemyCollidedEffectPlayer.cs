using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;

namespace Rogueskiv.Ux.EffectPlayers
{
    class EnemyCollidedEffectPlayer : EffectPlayer<EnemyCollidedEvent>
    {
        public EnemyCollidedEffectPlayer(RogueskivGame game, int channel = -1)
            : base(game, "enemy_collided", channel)
        { }

        protected override int GetVolume(EnemyCollidedEvent gameEvent) =>
            base.GetVolume(gameEvent) / 15;
    }
}
