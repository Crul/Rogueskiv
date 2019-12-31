using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;

namespace Rogueskiv.Ux.EffectPlayers
{
    class WinEffectPlayer : EventEffectPlayer<WinEvent>
    {
        public WinEffectPlayer(RogueskivGame game, int channel = -1)
            : base(game, "win", channel)
        { }

        protected override int GetVolume(WinEvent gameEvent) =>
            base.GetVolume(gameEvent) / 5;
    }
}
