using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;
using Seedwork.Ux;

namespace Rogueskiv.Ux.EffectPlayers
{
    class StairsDownEffectPlayer : EventEffectPlayer<StairsDownEvent>
    {
        public StairsDownEffectPlayer(UxContext uxContext, RogueskivGame game)
            : base(uxContext, game, "stairs_down") { }

        protected override int GetVolume(StairsDownEvent gameEvent) =>
            base.GetVolume(gameEvent) / 5;
    }
}
