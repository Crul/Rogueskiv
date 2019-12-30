using Rogueskiv.Core;
using Rogueskiv.Menus;
using Rogueskiv.Ux;
using Seedwork.Core.Entities;
using Seedwork.Engine;
using Seedwork.Ux;
using System.Collections.Generic;

namespace Rogueskiv.Run
{
    class RogueskivApp
    {
        private readonly RogueskivAppConfig AppConfig;
        private readonly GameStages<IEntity> GameStages = new GameStages<IEntity>();
        private readonly List<GameEngine<IEntity>> FloorEngines = new List<GameEngine<IEntity>>();
        private int CurrentFloor;

        public RogueskivApp(RogueskivAppConfig appConfig) => AppConfig = appConfig;

        public void Run()
        {
            using var uxContext = new UxContext("Rogueskiv", AppConfig);

            InitStages(uxContext);
            var engine = CreateMenuStage(uxContext);
            while (engine != null)
            {
                var gameResult = engine.RunLoop();
                engine = GameStages.GetNext(engine.Game.StageCode, gameResult);
            }
        }

        private void InitStages(UxContext uxContext)
        {
            GameStages.Add(
                (GameStageCodes.Menu, RogueskivMenuResults.PlayResult.ResultCode),
                result => CreateGameStage(uxContext)
            );
            GameStages.Add(
                (GameStageCodes.Game, RogueskivGameResults.FloorDown.ResultCode),
                result => GetFloorDown(uxContext, result)
            );
            GameStages.Add(
                (GameStageCodes.Game, RogueskivGameResults.FloorUp.ResultCode),
                result => GetFloorUp(result)
            );
            GameStages.Add(
                (GameStageCodes.Game, RogueskivGameResults.WinResult.ResultCode),
                result => CreateMenuStage(uxContext)
            );
            GameStages.Add(
                (GameStageCodes.Game, RogueskivGameResults.DeathResult.ResultCode),
                result => CreateMenuStage(uxContext)
            );
            GameStages.Add(
                (GameStageCodes.Game, null),
                result => CreateMenuStage(uxContext)
            );
        }

        private GameEngine<IEntity> GetFloorUp(IGameResult<IEntity> result) =>
            GetRestartFloorEngine(--CurrentFloor, result);

        private GameEngine<IEntity> GetFloorDown(UxContext uxContext, IGameResult<IEntity> result)
        {
            if (CurrentFloor < FloorEngines.Count)
                return GetRestartFloorEngine(++CurrentFloor, result);

            return CreateGameStage(uxContext, result);
        }

        private GameEngine<IEntity> GetRestartFloorEngine(int floor, IGameResult<IEntity> result)
        {
            var donwFloorEngine = FloorEngines[floor - 1];
            donwFloorEngine.Game.Restart(result);

            return donwFloorEngine;
        }

        private GameEngine<IEntity> CreateGameStage(UxContext uxContext, IGameResult<IEntity> result = null)
        {
            CurrentFloor = FloorEngines.Count + 1;

            var gameContext = new GameContext(AppConfig.MaxGameStepsWithoutRender);
            var gameConfig = new RogueskivGameConfig(AppConfig, gameContext, CurrentFloor);
            var game = new RogueskivGame(GameStageCodes.Game, gameConfig, result);
            var renderer = new RogueskivRenderer(uxContext, gameContext, game, AppConfig);
            var userInput = new RogueskivInputHandler(uxContext, game, renderer);
            var engine = new GameEngine<IEntity>(gameContext, userInput, game, renderer);

            FloorEngines.Add(engine);

            return engine;
        }

        private GameEngine<IEntity> CreateMenuStage(UxContext uxContext)
        {
            CurrentFloor = 1;
            FloorEngines.ForEach(gameEngine => gameEngine.Dispose());
            FloorEngines.Clear();

            var gameContext = new GameContext(AppConfig.MaxGameStepsWithoutRender);
            var game = new RogueskivMenu(GameStageCodes.Menu);
            var renderer = new RogueskivMenuRenderer(uxContext, game, AppConfig.FontFile);
            var userInput = new RogueskivMenuInputHandler(uxContext, game, renderer);
            var engine = new GameEngine<IEntity>(gameContext, userInput, game, renderer);

            return engine;
        }
    }
}
