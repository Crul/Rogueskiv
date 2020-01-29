using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;
using Seedwork.Ux;

namespace Rogueskiv.Ux.EffectPlayers
{
    class MapRevealerPickedEffectPlayer : EventEffectPlayer<MapRevealerPickedEvent>
    {
        public MapRevealerPickedEffectPlayer(UxContext uxContext, RogueskivGame game)
            : base(uxContext, game, "map_revealer_picked") { }

        protected override int GetVolume(MapRevealerPickedEvent gameEvent) =>
            base.GetVolume(gameEvent) / 2;
    }
}
