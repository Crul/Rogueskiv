﻿using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.Systems;
using Rogueskiv.MapGeneration;
using Seedwork.Core;
using Seedwork.Core.Components;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using Seedwork.Engine;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core
{
    public class RogueskivGame : Game
    {
        private bool HasStarted = false;
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
                    new List<IComponent> { new PopUpComp() { Text = GetStartText(floor) } },
                },
                systems: new List<ISystem> {
                    string.IsNullOrEmpty(boardData)
                        ? new BoardSys(GetMapGenerationParams(floorCount, floor))
                        : new BoardSys(boardData),
                    new SpawnSys(
                        gameContext,
                        (float)floor / floorCount,
                        previousFloorResult
                    ),
                    new PlayerSys(gameContext),
                    new MovementSys(),
                    new WallSys(),
                    new FoodSys(),
                    new TorchSys(),
                    new MapSys(),
                    new CollisionSys(),
                    new FOVSys(),
                    new StairsSys(floorCount == floor),
                    new DeathSys()
                },
                pauseControl: (int)Core.Controls.PAUSE,
                quitControl: (int)Core.Controls.QUIT
            )
        {
            Pause = true;
            BoardComp = Entities.GetSingleComponent<BoardComp>();
        }

        private static string GetStartText(int floor) =>
            $"FLOOR {floor + 1} - Press any arrow to " + (floor == 0 ? "start" : "continue");

        private static MapGenerationParams GetMapGenerationParams
            (int floorCount, int floor)
        {
            var floorFactor = (float)floor / floorCount;
            var mapSize = 48 + (int)(floorFactor * 16);           // 48    ... 64
            var roomExpandProb = 0.6f - (0.3f * floorFactor);     //  0.6  ...  0.3
            var corridorTurnProb = 0.05f + (0.15f * floorFactor); //  0.05 ...  0.2
            var minDensity = 0.18f - (0.12f * floorFactor);       //  0.18 ...  0.06
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

        public override void Restart(IGameResult<IEntity> previousFloorResult)
        {
            base.Restart(previousFloorResult);

            var playerEntity = Entities.GetWithComponent<PlayerComp>().Single();
            var previousPlayerEntity = previousFloorResult
                .Data
                .GetWithComponent<PlayerComp>()
                .Single();

            var playerMovementComp = playerEntity.GetComponent<MovementComp>();
            playerMovementComp.Stop();

            var playerComp = playerEntity.GetComponent<PlayerComp>();
            var previousPlayerComp = previousPlayerEntity.GetComponent<PlayerComp>();
            playerComp.VisualRange = previousPlayerComp.VisualRange;

            var previousPlayerHealtComp = previousPlayerEntity.GetComponent<HealthComp>();
            var playerHealthComp = playerEntity.GetComponent<HealthComp>();
            playerHealthComp.Health = previousPlayerHealtComp.Health;
        }

        public override void Update()
        {
            if (!HasStarted && Controls.Any())
            {
                Pause = false;
                HasStarted = true;
                Entities.GetSingleComponent<PopUpComp>().Text = "PAUSE";
            }

            base.Update();
        }

        public override void RemoveEntity(EntityId id)
        {
            var currentPositionComp = Entities[id].GetComponent<CurrentPositionComp>();
            BoardComp.RemoveEntity(id, currentPositionComp);
            base.RemoveEntity(id);
        }
    }
}
