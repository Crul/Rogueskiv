using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.GameEvents;
using Rogueskiv.Core.Systems;
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
        public int Floor { get; }
        public int GameSeed { get => GameConfig.GameSeed; }

        public List<IGameEvent> GameEvents { get; }

        private bool HasStarted = false;
        private readonly BoardComp BoardComp;
        private readonly TimerComp TimerComp;
        private readonly IRogueskivGameConfig GameConfig;

        public RogueskivGame(
            GameStageCode stageCode,
            IRogueskivGameConfig gameConfig,
            int floor,
            IGameResult<IEntity> previousFloorResult,
            string boardData = default
        )
            : base(
                stageCode: stageCode,
                entitiesComponents: new List<List<IComponent>>
                {
                    new List<IComponent> { new TimerComp(previousFloorResult?.Data.GetSingleComponent<TimerComp>()) },
                    new List<IComponent> { new BoardComp() },
                    new List<IComponent> { new FOVComp() },
                    new List<IComponent> { new PopUpComp() { Text = GetStartText(floor) } },
                },
                systems: new List<ISystem> {
                    new TimerSys(),
                    string.IsNullOrEmpty(boardData)
                        ? new BoardSys(gameConfig.GetMapGenerationParams(floor))
                        : new BoardSys(boardData),
                    new SpawnSys(floor, gameConfig, previousFloorResult),
                    new PlayerSys(gameConfig.PlayerAccelerationInGameTicks),
                    new MovementSys(),
                    new WallSys(),
                    new FoodSys(gameConfig.FoodHealthIncrease),
                    new TorchSys(gameConfig.TorchVisualRangeIncrease),
                    new RevealMapSys(),
                    new AmuletSys(),
                    new CollisionSys(gameConfig.EnemyCollisionDamage, gameConfig.EnemyCollisionBounce),
                    new FOVSys(),
                    new StairsSys(),
                    new DeathSys()
                },
                pauseControl: (int)Core.Controls.PAUSE,
                quitControl: (int)Core.Controls.QUIT
            )
        {
            Floor = floor;
            Pause = true;
            GameConfig = gameConfig;
            GameEvents = new List<IGameEvent>();
            BoardComp = Entities.GetSingleComponent<BoardComp>();
            TimerComp = Entities.GetSingleComponent<TimerComp>();
        }

        private static string GetStartText(int floor) =>
            $"FLOOR {floor}{Environment.NewLine}Press any arrow to " + (floor == 1 ? "start" : "continue");

        public override void Restart(IGameResult<IEntity> previousFloorResult)
        {
            base.Restart(previousFloorResult);

            var timerComp = Entities.GetSingleComponent<TimerComp>();
            var previousTimerComp = previousFloorResult.Data.GetSingleComponent<TimerComp>();
            timerComp.InGameTime = previousTimerComp.InGameTime;

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

        private bool HasCloseWindowBeenPressedLastTime = false;
        private bool HasToggleSoundsLastTime = false;

        public override void Update()
        {
            if (!HasStarted && Controls.Any())
            {
                Pause = false;
                HasStarted = true;
                var pauseText = $"PAUSE"
                    + $"{Environment.NewLine}"
                    + $"{Environment.NewLine}Controls:"
                    + $"{Environment.NewLine}ESC = continue playing"
                    + $"{Environment.NewLine}  Q = quit to menu    "
                    + $"{Environment.NewLine}  M = toggle music    "
                    + $"{Environment.NewLine}  S = toggle sounds   "
                    + $"{Environment.NewLine}"
                    + $"{Environment.NewLine}Seed: {GameConfig.GameSeed}";
                Entities.GetSingleComponent<PopUpComp>().Text = pauseText;
                if (!TimerComp.HasStarted) TimerComp.Start();
            }

            if (!Pause && Controls.Contains(QuitControl)) // only allow exit on pause
                Controls.Remove(QuitControl);

            var isCloseWindowBeenPressed = Controls.Contains((int)Core.Controls.CLOSE_WINDOW);
            if (isCloseWindowBeenPressed && !HasCloseWindowBeenPressedLastTime)
                if (Pause)
                    Quit = true;
                else
                    Pause = true;

            HasCloseWindowBeenPressedLastTime = isCloseWindowBeenPressed;

            var isToggleSoundPressed = Controls.Contains((int)Core.Controls.TOGGLE_SOUNDS);
            if (!HasToggleSoundsLastTime && isToggleSoundPressed)
                GameEvents.Add(new ToggleSoundEvent());

            HasToggleSoundsLastTime = isToggleSoundPressed;

            base.Update();
        }

        public override void EndGame(IGameResult<IEntity> gameResult, bool pauseBeforeQuit = false)
        {
            gameResult.Data.Add(Entities.GetWithComponent<TimerComp>().Single());
            base.EndGame(gameResult, pauseBeforeQuit);
        }

        public override void RemoveEntity(EntityId id)
        {
            var currentPositionComp = Entities[id].GetComponent<CurrentPositionComp>();
            BoardComp.RemoveEntity(id, currentPositionComp);
            base.RemoveEntity(id);
        }
    }
}
