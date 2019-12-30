using Rogueskiv.Core.Components;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using System.Collections.Generic;

namespace Rogueskiv.Core.Systems
{
    class TimerSys : BaseSystem
    {
        public override void Update(EntityList entities, List<int> controls) =>
            entities.GetSingleComponent<TimerComp>().InGameTime++;
    }
}
