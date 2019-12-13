using Rogueskiv.Core.Components;
using Rogueskiv.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    public class MovementSys : ISystem
    {
        public void Update(IList<IEntity> entities) =>
            entities
                .Where(e => e.HasComponent<MovementComp>())
                .ToList()
                .ForEach(Update);

        private void Update(IEntity entity)
        {
            var movement = entity.GetComponent<MovementComp>();
            var position = entity.GetComponent<PositionComp>();
            position.X += movement.SpeedX;
            position.Y += movement.SpeedY;
        }
    }
}
