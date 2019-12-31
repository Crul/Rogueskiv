using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;

namespace Rogueskiv.Ux.EffectPlayers
{
    class TorchPickedEffectPlayer : EventEffectPlayer<TorchPickedEvent>
    {
        public TorchPickedEffectPlayer(RogueskivGame game, int channel = -1)
            : base(game, "torch_picked", channel)
        { }
    }
}
