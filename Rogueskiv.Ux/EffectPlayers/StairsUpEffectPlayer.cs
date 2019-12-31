using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;

namespace Rogueskiv.Ux.EffectPlayers
{
    class StairsUpEffectPlayer : EventEffectPlayer<StairsUpEvent>
    {
        public StairsUpEffectPlayer(RogueskivGame game, int channel = -1)
            : base(game, "stairs_up", channel)
        { }

        protected override int GetVolume(StairsUpEvent gameEvent) =>
            base.GetVolume(gameEvent) / 10;
    }
}
