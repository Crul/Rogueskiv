using Rogueskiv.Core.Components;
using Seedwork.Core.Entities;
using System.Collections.Generic;

namespace Rogueskiv.Core.Systems
{
    class AmuletSys : PickingSys<AmuletComp>
    {
        public AmuletSys() : base(isSingleCompPerFloor: true) { }

        protected override void EndPicking(
            EntityList entities, List<(EntityId, AmuletComp)> pickedEntities
        )
        {
            base.EndPicking(entities, pickedEntities);
            Game.EndGame(RogueskivGameResults.WinResult, pauseBeforeQuit: true);
        }
    }
}
