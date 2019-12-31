using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;
using System.IO;

namespace Rogueskiv.Ux.EffectPlayers
{
    class TorchPickedEffectPlayer : EffectPlayer<TorchPickedEvent>
    {
        public TorchPickedEffectPlayer(RogueskivGame game, int channel = -1)
            : base(game, Path.Combine("audio", "torch_picked.mp3"), channel)
        { }

        protected override int GetVolume(TorchPickedEvent gameEvent) =>
            base.GetVolume(gameEvent) / 10;
    }
}
