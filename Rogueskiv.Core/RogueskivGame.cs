using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.Systems;
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
            GameStageCode stageCode,
            IRogueskivGameConfig gameConfig,
            IGameResult<IEntity> previousFloorResult,
            string boardData = default
        )
            : base(
                stageCode: stageCode,
                entitiesComponents: new List<List<IComponent>>
                {
                    new List<IComponent> { new BoardComp() },
                    new List<IComponent> { new FOVComp() },
                    new List<IComponent> { new PopUpComp() { Text = GetStartText(gameConfig.Floor) } },
                },
                systems: new List<ISystem> {
                    string.IsNullOrEmpty(boardData)
                        ? new BoardSys(gameConfig.MapGenerationParams)
                        : new BoardSys(boardData),
                    new SpawnSys(
                        gameConfig,
                        previousFloorResult
                    ),
                    new PlayerSys(gameConfig.PlayerAcceleration),
                    new MovementSys(),
                    new WallSys(),
                    new FoodSys(gameConfig.MaxItemPickingTime, gameConfig.FoodHealthIncrease),
                    new TorchSys(gameConfig.MaxItemPickingTime, gameConfig.TorchVisualRangeIncrease),
                    new RevealMapSys(gameConfig.MaxItemPickingTime),
                    new AmuletSys(gameConfig.MaxItemPickingTime),
                    new CollisionSys(gameConfig.EnemyCollisionDamage),
                    new FOVSys(),
                    new StairsSys(),
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
            $"FLOOR {floor} - Press any arrow to " + (floor == 1 ? "start" : "continue");

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
