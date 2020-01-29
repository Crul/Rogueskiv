using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;
using Seedwork.Ux;

namespace Rogueskiv.Ux.EffectPlayers
{
    class WinEffectPlayer : EventEffectPlayer<WinEvent>
    {
        public WinEffectPlayer(UxContext uxContext, RogueskivGame game)
            : base(uxContext, game, "win") { }
    }
}
