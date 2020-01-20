using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;
using Seedwork.Ux;

namespace Rogueskiv.Ux.EffectPlayers
{
    class StairsUpEffectPlayer : EventEffectPlayer<StairsUpEvent>
    {
        public StairsUpEffectPlayer(UxContext uxContext, RogueskivGame game)
            : base(uxContext, game, "stairs_up") { }

        protected override int GetVolume(StairsUpEvent gameEvent) =>
            base.GetVolume(gameEvent) / 5;
    }
}
