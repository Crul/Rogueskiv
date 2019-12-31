using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;

namespace Rogueskiv.Ux.EffectPlayers
{
    class TorchPickedEffectPlayer : EffectPlayer<TorchPickedEvent>
    {
        public TorchPickedEffectPlayer(RogueskivGame game, int channel = -1)
            : base(game, "torch_picked", channel)
        { }

        protected override int GetVolume(TorchPickedEvent gameEvent) =>
            base.GetVolume(gameEvent) / 10;
    }
}
