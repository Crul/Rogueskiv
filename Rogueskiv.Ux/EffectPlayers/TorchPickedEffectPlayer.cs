using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;
using Seedwork.Ux;

namespace Rogueskiv.Ux.EffectPlayers
{
    class TorchPickedEffectPlayer : EventEffectPlayer<TorchPickedEvent>
    {
        public TorchPickedEffectPlayer(UxContext uxContext, RogueskivGame game)
            : base(uxContext, game, "torch_picked") { }
    }
}
