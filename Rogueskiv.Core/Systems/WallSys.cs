using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.Components.Walls;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    public class WallSys : BaseSystem
    {
        public override void Update(List<IEntity> entities, IEnumerable<int> controls)
        {

            var wallComponents = entities
                .GetWithComponent<IWallComp>()
                .Select(wall => wall.GetComponent<IWallComp>());

            entities
                .Where(e => e.HasComponent<LastPositionComp>())
                .ToList()
                .ForEach(entity => Update(entity, wallComponents));
        }

        private void Update(IEntity entity, IEnumerable<IWallComp> wallComponents)
        {
            var lastPosition = entity.GetComponent<LastPositionComp>();
            var position = entity.GetComponent<CurrentPositionComp>();
            var movement = entity.GetComponent<MovementComp>();

            int tmpCheck = 0; // TODO 
            bool checkBounces;
            do
            {
                tmpCheck++;
                checkBounces = wallComponents
                    .Any(wallComp => wallComp.CheckBounce(movement, position, lastPosition));

                if (tmpCheck > 10)
                    throw new System.Exception("TOO MANY BOUNCES!!!");

            } while (checkBounces);
        }
    }
}
