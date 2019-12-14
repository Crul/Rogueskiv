using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Walls;
using Rogueskiv.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    public class WallSys
    {
        public void Update(
            List<IEntity> entities,
            MovementComp movement,
            PositionComp position,
            PositionComp oldPosition
        )
        {
            var wallComponents = entities
                .GetWithComponent<IWallComp>()
                .Select(wall => wall.GetComponent<IWallComp>());

            int tmpCheck = 0; // TODO 
            bool checkBounces;
            do
            {
                tmpCheck++;
                checkBounces = wallComponents
                    .Any(wallComp => wallComp.CheckBounce(movement, position, oldPosition));

                if (tmpCheck > 10)
                    throw new System.Exception("TOO MANY BOUNCES!!!");

            } while (checkBounces);
        }
    }
}
