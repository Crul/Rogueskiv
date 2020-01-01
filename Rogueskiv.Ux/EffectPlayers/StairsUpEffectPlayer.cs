using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;

namespace Rogueskiv.Ux.EffectPlayers
{
    class StairsUpEffectPlayer : EventEffectPlayer<StairsUpEvent>
    {
        public StairsUpEffectPlayer(RogueskivGame game) : base(game, "stairs_up") { }

        protected override int GetVolume(StairsUpEvent gameEvent) =>
            base.GetVolume(gameEvent) / 5;
    }
}
