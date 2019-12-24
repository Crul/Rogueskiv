
using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    class MapSys : BaseSystem
    {
        private Game Game;
        private PositionComp PlayerPositionComp;
        private List<TileComp> TileComps;
        private List<StairsComp> StairsComps;

        public override void Init(Game game)
        {
            Game = game;
            PlayerPositionComp = game.Entities.GetSingleComponent<PlayerComp, CurrentPositionComp>();
            TileComps = game.Entities.GetComponents<TileComp>();
            StairsComps = game.Entities.GetComponents<StairsComp>();
        }

        public override void Update(EntityList entities, List<int> controls)
        {
            // TODO DRY FoodSys and TorchSys (... PickableSys ?)

            var pickedMaps = entities
                .GetWithComponent<MapComp>()
                .Select(e => (
                    mapEntityId: e.Id,
                    mapComp: e.GetComponent<MapComp>()
                ))
                .Where(map => map.mapComp.TilePos == PlayerPositionComp.TilePos)
                .ToList();

            if (pickedMaps.Any())
            {
                TileComps.ForEach(tileComp => tileComp.HasBeenSeen = true);
                StairsComps.ForEach(stairsComp => stairsComp.HasBeenSeen = true);
                pickedMaps.ForEach(map => Game.RemoveEntity(map.mapEntityId));
                Game.RemoveSystem(this);  // only one map per floor
            }
        }
    }
}
