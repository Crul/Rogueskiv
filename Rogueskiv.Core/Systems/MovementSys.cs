using Rogueskiv.Core.Components;
using Rogueskiv.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    public class MovementSys : BaseSystem
    {
        private WallSys WallSystem;

        public MovementSys() => WallSystem = new WallSys();

        public override void Update(List<IEntity> entities) =>
            entities
                .Where(e => e.HasComponent<MovementComp>())
                .ToList()
                .ForEach(e => Update(entities, e));

        private void Update(List<IEntity> entities, IEntity entity)
        {
            var movement = entity.GetComponent<MovementComp>();
            var position = entity.GetComponent<PositionComp>();

            var oldPosition = position.Clone();
            position.X += movement.SpeedX;
            position.Y += movement.SpeedY;

            WallSystem.Update(entities, movement, position, oldPosition);
        }
    }
}
