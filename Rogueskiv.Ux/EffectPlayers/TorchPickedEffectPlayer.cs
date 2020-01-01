using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;

namespace Rogueskiv.Ux.EffectPlayers
{
    class TorchPickedEffectPlayer : EventEffectPlayer<TorchPickedEvent>
    {
        public TorchPickedEffectPlayer(RogueskivGame game) : base(game, "torch_picked") { }
    }
}
