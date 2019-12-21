using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Position;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using System.Collections.Generic;
using System.Drawing;

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

            positionComp.Position = new PointF(
                positionComp.Position.X + movementComp.Speed.X,
                positionComp.Position.Y + movementComp.Speed.Y
            );
        }
    }
}
