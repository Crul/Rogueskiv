using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.Systems;
using Rogueskiv.MapGeneration;
using Seedwork.Core;
using Seedwork.Core.Components;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using Seedwork.Engine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core
{
    public class RogueskivGame : Game
    {
        private readonly BoardComp BoardComp;

        public RogueskivGame(
            IGameContext gameContext,
            GameStageCode stageCode,
            int floorCount,
            int floor,
            IGameResult<IEntity> previousFloorResult,
            string boardData = default
        )
            : base(
                stageCode: stageCode,
                entitiesComponents: new List<List<IComponent>>
                {
                    new List<IComponent> { new BoardComp() },
                    new List<IComponent> { new FOVComp() },
                    new List<IComponent> { new PopUpComp() },
                },
                systems: new List<ISystem> {
                    string.IsNullOrEmpty(boardData)
                        ? new BoardSys(GetMapGenerationParams(floorCount, floor))
                        : new BoardSys(boardData),
                    new SpawnSys(
                        gameContext,
                        previousFloorResult,
                        GetEnemyNumber(floorCount, floor)
                    ),
                    new PlayerSys(gameContext),
                    new MovementSys(),
                    new WallSys(),
                    new CollisionSys(),
                    new FOVSys(),
                    new StairsSys(floorCount == floor),
                    new DeathSys()
                },
                pauseControl: (int)Core.Controls.PAUSE,
                quitControl: (int)Core.Controls.QUIT
            ) => BoardComp = Entities
                    .GetWithComponent<BoardComp>()
                    .Single()
                    .GetComponent<BoardComp>();

        private static int GetEnemyNumber(int floorCount, int floor) =>
            (int)Math.Pow(5 + (15 * ((float)floor / floorCount)), 1.5f);  // 11 ... 103

        private static MapGenerationParams GetMapGenerationParams
            (int floorCount, int floor)
        {
            var floorFactor = (float)floor / floorCount;
            var mapSize = 32 + (int)(floorFactor * 32);           // 32    ... 64
            var roomExpandProb = 0.6f - (0.3f * floorFactor);     //  0.6  ...  0.3
            var corridorTurnProb = 0.05f + (0.15f * floorFactor); //  0.05 ...  0.2
            var minDensity = 0.18f - (0.12f * floorFactor);       //  0.18 ...  0.06
            var initialRooms = 15 + (int)(floorFactor * 45);      // 15    ... 60
            var minRoomSize = 2;                                  //  2

            return new MapGenerationParams(
                width: mapSize,
                height: mapSize,
                roomExpandProb,
                corridorTurnProb,
                minDensity,
                initialRooms,
                minRoomSize
            );
        }

        public override void Restart(IGameResult<IEntity> previousFloorResult)
        {
            base.Restart(previousFloorResult);

            var playerComp = Entities
                .GetWithComponent<PlayerComp>()
                .Single();

            var playerMovementComp = playerComp.GetComponent<MovementComp>();
            playerMovementComp.Stop();

            var previousPlayerComp = previousFloorResult
                .Data
                .GetWithComponent<PlayerComp>()
                .Single();

            var previousPlayerHealtComp = previousPlayerComp.GetComponent<HealthComp>();
            var playerHealthComp = playerComp.GetComponent<HealthComp>();

            playerHealthComp.Health = previousPlayerHealtComp.Health;
        }

        public override void RemoveEntity(EntityId id)
        {
            var position = Entities[id].GetComponent<CurrentPositionComp>();
            BoardComp.RemoveEntity(id, position);
            base.RemoveEntity(id);
        }
    }
}
