using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Seedwork.Core;
using Seedwork.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    class RevealMapSys : PickingSys<MapRevealerComp>
    {
        private FOVComp FOVComp;
        private BoardComp BoardComp;

        public RevealMapSys() : base(isSingleCompPerFloor: true) { }

        public override void Init(Game game)
        {
            base.Init(game);
            FOVComp = game.Entities.GetSingleComponent<FOVComp>();
            BoardComp = game.Entities.GetSingleComponent<BoardComp>();
        }

        protected override void StartPicking(List<MapRevealerComp> pickedMaps)
        {
            base.StartPicking(pickedMaps);
            if (pickedMaps.Any())
                FOVComp.RevealAll(BoardComp);
        }
    }
}
