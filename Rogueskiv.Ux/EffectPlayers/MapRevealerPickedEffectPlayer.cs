using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;

namespace Rogueskiv.Ux.EffectPlayers
{
    class MapRevealerPickedEffectPlayer : EventEffectPlayer<MapRevealerPickedEvent>
    {
        public MapRevealerPickedEffectPlayer(RogueskivGame game, int channel = -1)
            : base(game, "map_revealer_picked", channel)
        { }

        protected override int GetVolume(MapRevealerPickedEvent gameEvent) =>
            base.GetVolume(gameEvent) / 2;
    }
}
