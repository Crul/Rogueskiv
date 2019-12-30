using Rogueskiv.Core;
using Rogueskiv.Menus;
using Rogueskiv.Ux;
using Seedwork.Core.Entities;
using Seedwork.Engine;
using Seedwork.Ux;
using System;
using System.Collections.Generic;

namespace Rogueskiv.Run
{
    class RogueskivApp : IDisposable
    {
        private readonly UxContext UxContext;
        private readonly RogueskivAppConfig AppConfig;
        private readonly GameStages<IEntity> GameStages = new GameStages<IEntity>();
        private readonly List<GameEngine<IEntity>> FloorEngines = new List<GameEngine<IEntity>>();
        private readonly LoadingScreenRenderer LoadingScreenRenderer;

        private int CurrentFloor;

        public RogueskivApp(RogueskivAppConfig appConfig)
        {
            AppConfig = appConfig;
            UxContext = new UxContext("Rogueskiv", AppConfig);
            LoadingScreenRenderer = new LoadingScreenRenderer(UxContext, AppConfig.FontFile);
        }

        public void Run()
        {
            InitStages();
            var engine = CreateMenuStage();
            while (engine != null)
            {
                var gameResult = engine.RunLoop();
                engine = GameStages.GetNext(engine.Game.StageCode, gameResult);
            }
        }

        private void InitStages()
        {
            GameStages.Add(
                (GameStageCodes.Menu, RogueskivMenuResults.PlayResult.ResultCode),
                result => CreateGameStage()
            );
            GameStages.Add(
                (GameStageCodes.Game, RogueskivGameResults.FloorDown.ResultCode),
                result => GetFloorDown(result)
            );
            GameStages.Add(
                (GameStageCodes.Game, RogueskivGameResults.FloorUp.ResultCode),
                result => GetFloorUp(result)
            );
            GameStages.Add(
                (GameStageCodes.Game, RogueskivGameResults.WinResult.ResultCode),
                result => CreateMenuStage()
            );
            GameStages.Add(
                (GameStageCodes.Game, RogueskivGameResults.DeathResult.ResultCode),
                result => CreateMenuStage()
            );
            GameStages.Add(
                (GameStageCodes.Game, null),
                result => CreateMenuStage()
            );
        }

        private GameEngine<IEntity> GetFloorUp(IGameResult<IEntity> result) =>
            GetRestartFloorEngine(--CurrentFloor, result);

        private GameEngine<IEntity> GetFloorDown(IGameResult<IEntity> result)
        {
            if (CurrentFloor < FloorEngines.Count)
                return GetRestartFloorEngine(++CurrentFloor, result);

            return CreateGameStage(result);
        }

        private GameEngine<IEntity> GetRestartFloorEngine(int floor, IGameResult<IEntity> result)
        {
            var donwFloorEngine = FloorEngines[floor - 1];
            donwFloorEngine.Game.Restart(result);

            return donwFloorEngine;
        }

        private GameEngine<IEntity> CreateGameStage(IGameResult<IEntity> result = null)
        {
            LoadingScreenRenderer.Render();

            CurrentFloor = FloorEngines.Count + 1;

            var gameContext = new GameContext(AppConfig.MaxGameStepsWithoutRender);
            var gameConfig = new RogueskivGameConfig(AppConfig, gameContext, CurrentFloor);
            var game = new RogueskivGame(GameStageCodes.Game, gameConfig, result);
            var renderer = new RogueskivRenderer(UxContext, gameContext, game, AppConfig);
            var userInput = new RogueskivInputHandler(UxContext, game, renderer);
            var engine = new GameEngine<IEntity>(gameContext, userInput, game, renderer);

            FloorEngines.Add(engine);

            return engine;
        }

        private GameEngine<IEntity> CreateMenuStage()
        {
            CurrentFloor = 1;
            FloorEngines.ForEach(gameEngine => gameEngine.Dispose());
            FloorEngines.Clear();

            var gameContext = new GameContext(AppConfig.MaxGameStepsWithoutRender);
            var game = new RogueskivMenu(GameStageCodes.Menu);
            var renderer = new RogueskivMenuRenderer(UxContext, game, AppConfig.FontFile);
            var userInput = new RogueskivMenuInputHandler(UxContext, game, renderer);
            var engine = new GameEngine<IEntity>(gameContext, userInput, game, renderer);

            return engine;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool cleanManagedResources)
        {
            if (cleanManagedResources)
            {
                LoadingScreenRenderer.Dispose();
                FloorEngines.ForEach(gameEngine => gameEngine.Dispose());
                UxContext.Dispose();
            }
        }
    }
}
