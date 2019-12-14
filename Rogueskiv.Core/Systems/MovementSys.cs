using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Position;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    public class MovementSys : BaseSystem
    {
        public override void Update(List<IEntity> entities, IEnumerable<int> controls) =>
            entities
                .Where(e => e.HasComponent<MovementComp>())
                .ToList()
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
