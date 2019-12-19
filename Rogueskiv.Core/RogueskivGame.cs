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
            string boardData = default
        )
            : base(
                quitControl: (int)Core.Controls.QUIT,
                stageCode: stageCode,
                entitiesComponents: new List<List<IComponent>>
                {
                    new List<IComponent> { new BoardComp() }
                },
                systems: new List<ISystem> {
                    string.IsNullOrEmpty(boardData)
                        ? new BoardSys(GetMapGenerationParams(floorCount, floor))
                        : new BoardSys(boardData),
                    new SpawnSys(
                        gameContext,
                        GetEnemyNumber(floorCount, floor),
                        isFirstFloor: floor == 0
                    ),
                    new PlayerSys(gameContext),
                    new MovementSys(),
                    new WallSys(),
                    new CollisionSys(),
                    new FOVSys(),
                    new StairsSys(),
                    new DeathSys()
                }
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
            var roomExpandProb = 0.3f - (0.25f * floorFactor);    //  0.3  ...  0.05
            var corridorTurnProb = 0.05f + (0.15f * floorFactor); //  0.05 ...  0.2
            var minDensity = 0.12f - (0.06f * floorFactor);       //  0.12 ...  0.06
            var initialRooms = 15 + (int)(floorFactor * 45);      // 15    ... 60
            var minRoomSize = 3;                                  //  3

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

        public override void Restart()
        {
            base.Restart();

            var playerMovementComp = Entities
                .GetWithComponent<PlayerComp>()
                .Single()
                .GetComponent<MovementComp>();

            playerMovementComp.SpeedX = 0;
            playerMovementComp.SpeedY = 0;
        }

        public override void RemoveEntity(EntityId id)
        {
            var position = Entities[id].GetComponent<CurrentPositionComp>();
            BoardComp.RemoveEntity(id, position);
            base.RemoveEntity(id);
        }
    }
}
