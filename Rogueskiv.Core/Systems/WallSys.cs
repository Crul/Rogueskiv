using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.Components.Walls;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    public class WallSys : BaseSystem
    {
        private BoardComp BoardComp;

        public override void Init(Game game) =>
            BoardComp = game
                .Entities
                .GetWithComponent<BoardComp>()
                .Single()
                .GetComponent<BoardComp>();

        public override void Update(EntityList entities, List<int> controls) =>
            entities
                .GetWithComponent<LastPositionComp>()
                .ForEach(entity => Update(entity, entities));

        private void Update(IEntity entity, EntityList entities)
        {
            var lastPosition = entity.GetComponent<LastPositionComp>();
            var positionComp = entity.GetComponent<CurrentPositionComp>();
            var movementComp = entity.GetComponent<MovementComp>();

            var wallComponents = BoardComp
                .GetWallsIdsNear(entity.Id, positionComp)
                .Select(wei => entities[wei])
                .Select(wall => wall.GetComponent<WallComp>())
                .ToList();

            int tmpCheck = 0; // TODO 
            bool checkBounces;
            do
            {
                tmpCheck++;
                checkBounces = wallComponents
                    .Any(wallComp => wallComp.CheckBounce(movementComp, positionComp, lastPosition));

                if (tmpCheck > 10)
                    throw new System.Exception("TOO MANY BOUNCES!!!");

            } while (checkBounces);
        }
    }
}
