using Rogueskiv.Core.Components;
using Seedwork.Core;
using Seedwork.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    class MapSys : PickingSys<MapComp>
    {
        private FOVComp FOVComp;

        public MapSys() : base(isSingleCompPerFloor: true) { }

        public override void Init(Game game)
        {
            base.Init(game);
            FOVComp = game.Entities.GetSingleComponent<FOVComp>();
        }

        protected override void StartPicking(List<MapComp> pickedMaps)
        {
            base.StartPicking(pickedMaps);
            if (pickedMaps.Any())
                FOVComp.RevealAll();
        }
    }
}
