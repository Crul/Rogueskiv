using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Position;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using Seedwork.Crosscutting;
using System.Collections.Generic;

namespace Rogueskiv.Core.Systems
{
    public class MovementSys : BaseSystem
    {
        public override void Update(EntityList entities, List<int> controls) =>
            entities
                .GetWithComponent<MovementComp>()
                .ForEach(Update);

        private void Update(IEntity entity)
        {
            var lastPositionComp = entity.GetComponent<LastPositionComp>();
            var positionComp = entity.GetComponent<CurrentPositionComp>();
            var movementComp = entity.GetComponent<MovementComp>();

            lastPositionComp.Position = positionComp.Position;
            positionComp.Position = positionComp.Position.Add(movementComp.Speed);
        }
    }
}
