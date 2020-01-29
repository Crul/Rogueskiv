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
        private const long TICKS_IN_A_SECOND = 10000000;

        public int Floor { get; }
        public int GameSeed { get => GameConfig.GameSeed; }

        public List<IGameEvent> GameEvents { get; }

        public RogueskivGameStats GameStats { get; private set; }

        private bool HasStarted = false;
        private readonly BoardComp BoardComp;
        private readonly TimerComp TimerComp;
        private readonly IRogueskivGameConfig GameConfig;
        private readonly Action<RogueskivGameStats> OnGameEnd;

        public RogueskivGame(
            GameStageCode stageCode,
            IRogueskivGameConfig gameConfig,
            int floor,
            IGameResult<EntityList> previousFloorResult,
            Action<RogueskivGameStats> onGameEnd,
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
            OnGameEnd = onGameEnd;
        }

        private static string GetStartText(int floor) =>
            $"FLOOR {floor}{Environment.NewLine}Press any arrow to " + (floor == 1 ? "start" : "continue");

        public override void Restart(IGameResult<EntityList> previousFloorResult)
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
                    + $"{Environment.NewLine}  N = toggle sounds   "
                    + $"{Environment.NewLine}  M = toggle music    "
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

        protected override void UpdateSystems()
        {
            if (Result?.ResultCode == RogueskivGameResults.DeathResult.ResultCode)
                Quit = true;
            else
                base.UpdateSystems();
        }

        public override void EndGame(IGameResult<EntityList> gameResult, bool pauseBeforeQuit = false)
        {
            var timerEntity = Entities.GetWithComponent<TimerComp>().Single();
            var isEndGameResult = (
                gameResult?.ResultCode == RogueskivGameResults.DeathResult.ResultCode
                || gameResult?.ResultCode == RogueskivGameResults.WinResult.ResultCode
            );
            if (isEndGameResult)
            {
                SetGameStats(gameResult, timerEntity.GetComponent<TimerComp>());
                OnGameEnd(GameStats);
            }

            gameResult.Data.Add(timerEntity.Id, timerEntity);
            base.EndGame(gameResult, pauseBeforeQuit);
        }

        public override void RemoveEntity(EntityId id)
        {
            var currentPositionComp = Entities[id].GetComponent<CurrentPositionComp>();
            BoardComp.RemoveEntity(id, currentPositionComp);
            base.RemoveEntity(id);
        }

        private void SetGameStats(IGameResult<EntityList> gameResult, TimerComp timerComp)
        {
            GameStats = new RogueskivGameStats()
            {
                Timestamp = timerComp.RealTimeStart.Value.Ticks,
                DiedOnFloor = gameResult?.ResultCode == RogueskivGameResults.DeathResult.ResultCode ? (int?)Floor : null,
                FinalHealth = Entities.GetSingleComponent<HealthComp>().Health,
                RealTime = timerComp.GetRealTime().Ticks,
                InGameTime = TICKS_IN_A_SECOND * timerComp.InGameTime / GameConfig.GameFPS,
            };
        }
    }
}
