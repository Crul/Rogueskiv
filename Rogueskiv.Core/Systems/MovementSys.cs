using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Position;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
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
            var lastPosition = entity.GetComponent<LastPositionComp>();
            var position = entity.GetComponent<CurrentPositionComp>();
            var movement = entity.GetComponent<MovementComp>();

            lastPosition.X = position.X;
            lastPosition.Y = position.Y;

            position.X += movement.SpeedX;
            position.Y += movement.SpeedY;
        }
    }
}
