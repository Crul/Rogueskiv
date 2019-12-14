using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
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
        public override void Update(EntityList entities, IEnumerable<int> controls)
        {

            var boardComp = entities
                .GetWithComponent<BoardComp>()
                .Single()
                .GetComponent<BoardComp>();

            entities
                .GetWithComponent<LastPositionComp>()
                .ForEach(entity => Update(entity, boardComp, entities));
        }

        private void Update(IEntity entity, BoardComp boardComp, EntityList entities)
        {
            var lastPosition = entity.GetComponent<LastPositionComp>();
            var position = entity.GetComponent<CurrentPositionComp>();
            var movement = entity.GetComponent<MovementComp>();

            var wallEntityIds = boardComp.GetWallsIdsNear(position);

            var wallComponents = wallEntityIds
                .Select(wei => entities[wei])
                .Select(wall => wall.GetComponent<WallComp>());

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
