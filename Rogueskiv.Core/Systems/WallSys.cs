using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.Components.Walls;
using Rogueskiv.Core.GameEvents;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using Seedwork.Crosscutting;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    public class WallSys : BaseSystem
    {
        private RogueskivGame Game;
        private BoardComp BoardComp;

        public override void Init(Game game)
        {
            Game = (RogueskivGame)game;
            BoardComp = game.Entities.GetSingleComponent<BoardComp>();
        }

        public override void Update(EntityList entities, List<int> controls) =>
            entities
                .GetWithComponent<LastPositionComp>()
                .ForEach(entity => Update(entity, entities));

        private void Update(IEntity entity, EntityList entities)
        {
            var lastPositionComp = entity.GetComponent<LastPositionComp>();
            var positionComp = entity.GetComponent<CurrentPositionComp>();
            var movementComp = entity.GetComponent<MovementComp>();

            var wallComponents = BoardComp
                .GetWallsIdsNear(lastPositionComp)
                .Select(wei => entities[wei])
                .Select(wall => wall.GetComponent<WallComp>())
                .ToList();

            int tmpCheck = 0; // TODO 
            bool checkBounces;
            do
            {
                tmpCheck++;
                var lastMovementDistance =
                    GetLastMovementDistanceIfPlayer(movementComp, positionComp, lastPositionComp);

                movementComp.HasBounced = false;
                checkBounces = wallComponents
                    .Any(wallComp => wallComp.CheckBounce(movementComp, positionComp, lastPositionComp));

                AddPlayerHitWallEvent(movementComp, lastMovementDistance);

                if (tmpCheck > 10)
                    throw new System.Exception("TOO MANY BOUNCES!!!");

            } while (checkBounces);
        }

        private static float GetLastMovementDistanceIfPlayer(
            MovementComp movementComp,
            CurrentPositionComp positionComp,
            LastPositionComp lastPositionComp
        ) =>
            movementComp.SimpleBounce
                ? 0
                : Distance.Get(
                    lastPositionComp.Position.Substract(positionComp.Position)
                );

        private void AddPlayerHitWallEvent(MovementComp movementComp, float lastMovementDistance)
        {
            if (!movementComp.HasBounced || movementComp.SimpleBounce) // TODO identify player better
                return;

            var speedFactor = lastMovementDistance / ((BoundedMovementComp)movementComp).MaxSpeed;
            Game.GameEvents.Add(new PlayerHitWallEvent(speedFactor));
        }
    }
}
