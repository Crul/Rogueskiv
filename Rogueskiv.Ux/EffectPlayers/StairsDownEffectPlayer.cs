using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;

namespace Rogueskiv.Ux.EffectPlayers
{
    class StairsDownEffectPlayer : EventEffectPlayer<StairsDownEvent>
    {
        public StairsDownEffectPlayer(RogueskivGame game, int channel = -1)
            : base(game, "stairs_down", channel)
        { }

        protected override int GetVolume(StairsDownEvent gameEvent) =>
            base.GetVolume(gameEvent) / 5;
    }
}
