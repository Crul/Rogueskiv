
using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Seedwork.Core;
using Seedwork.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    class MapSys : PickingSys<MapComp>
    {
        private List<TileComp> TileComps;
        private List<StairsComp> StairsComps;

        public MapSys() : base(isSingleCompPerFloor: true) { }

        public override void Init(Game game)
        {
            base.Init(game);

            TileComps = game.Entities.GetComponents<TileComp>();
            StairsComps = game.Entities.GetComponents<StairsComp>();
        }

        protected override void StartPicking(List<MapComp> pickedMaps)
        {
            base.StartPicking(pickedMaps);
            if (pickedMaps.Any())
            {
                TileComps.ForEach(tileComp => tileComp.HasBeenSeen = true);
                StairsComps.ForEach(stairsComp => stairsComp.HasBeenSeen = true);
            }
        }
    }
}
