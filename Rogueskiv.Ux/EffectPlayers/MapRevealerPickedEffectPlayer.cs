using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;

namespace Rogueskiv.Ux.EffectPlayers
{
    class MapRevealerPickedEffectPlayer : EventEffectPlayer<MapRevealerPickedEvent>
    {
        public MapRevealerPickedEffectPlayer(RogueskivGame game)
            : base(game, "map_revealer_picked")
        { }

        protected override int GetVolume(MapRevealerPickedEvent gameEvent) =>
            base.GetVolume(gameEvent) / 2;
    }
}
