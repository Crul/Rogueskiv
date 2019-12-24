using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Position;
using Seedwork.Core;
using Seedwork.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    class MapSys : PickingSys<MapComp>
    {
        private List<RevealableByMapPositionComp> RevealableByMapPositionComps;

        public MapSys() : base(isSingleCompPerFloor: true) { }

        public override void Init(Game game)
        {
            base.Init(game);
            RevealableByMapPositionComps = game.Entities.GetComponents<RevealableByMapPositionComp>();
        }

        protected override void StartPicking(List<MapComp> pickedMaps)
        {
            base.StartPicking(pickedMaps);
            if (pickedMaps.Any())
                RevealableByMapPositionComps
                    .ForEach(revealableComp => revealableComp.RevealedByMap = true);
        }
    }
}
