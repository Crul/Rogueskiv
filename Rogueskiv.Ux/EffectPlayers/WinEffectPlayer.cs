using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;

namespace Rogueskiv.Ux.EffectPlayers
{
    class WinEffectPlayer : EventEffectPlayer<WinEvent>
    {
        public WinEffectPlayer(RogueskivGame game, int channel = -1)
            : base(game, "win", channel)
        { }
    }
}
