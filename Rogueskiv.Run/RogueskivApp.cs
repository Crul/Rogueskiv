﻿using Rogueskiv.Core;
using Rogueskiv.Menus;
using Rogueskiv.Ux;
using Seedwork.Core.Entities;
using Seedwork.Crosscutting;
using Seedwork.Engine;
using Seedwork.Ux;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Run
{
    class RogueskivApp : IDisposable
    {
        private readonly UxContext UxContext;
        private readonly GameContext GameContext;
        private readonly RogueskivAppConfig AppConfig;
        private readonly GameStages<IEntity> GameStages = new GameStages<IEntity>();
        private readonly List<GameEngine<IEntity>> FloorEngines = new List<GameEngine<IEntity>>();
        private readonly LoadingScreenRenderer LoadingScreenRenderer;

        private int CurrentFloor;

        public RogueskivApp(RogueskivAppConfig appConfig)
        {
            AppConfig = appConfig;
            UxContext = new UxContext("Rogueskiv", AppConfig, imagesPath: "imgs", audiosPath: "audio", fontsPath: "fonts");
            GameContext = new GameContext(appConfig.MaxGameStepsWithoutRender);
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
                result => CreateGameStage(gameSeed: ((PlayGameResult)result).GameSeed)
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

        private GameEngine<IEntity> GetFloorUp(IGameResult<IEntity> result)
        {
            StopCurrentFloorEngine();

            return GetRestartFloorEngine(--CurrentFloor, result);
        }

        private GameEngine<IEntity> GetFloorDown(IGameResult<IEntity> result)
        {
            StopCurrentFloorEngine();
            if (CurrentFloor < FloorEngines.Count)
                return GetRestartFloorEngine(++CurrentFloor, result);

            return CreateGameStage(result);
        }

        private GameEngine<IEntity> GetRestartFloorEngine(int floor, IGameResult<IEntity> result)
        {
            var floorEngine = FloorEngines[floor - 1];
            floorEngine.Game.Restart(result);

            return floorEngine;
        }

        private GameEngine<IEntity> CreateGameStage(
            IGameResult<IEntity> result = null,
            int? gameSeed = null
        )
        {
            LoadingScreenRenderer.Render();

            if (gameSeed.HasValue)
                GameContext.SetSeed(gameSeed.Value);

            var gameConfig = YamlParser.ParseFile<RogueskivGameConfig>(AppConfig.GameModeFilesPath, AppConfig.GameMode);
            gameConfig.GameFPS = GameContext.GameFPS;
            gameConfig.GameSeed = GameContext.GameSeed;
            gameConfig.FloorCount = AppConfig.FloorCount;

            if (FloorEngines.Count == 0)
                UxContext.PlayMusic(gameConfig.GameMusicFilePath, gameConfig.GameMusicVolume);

            CurrentFloor = FloorEngines.Count + 1;
            var game = new RogueskivGame(GameStageCodes.Game, gameConfig, CurrentFloor, result);
            var renderer = new RogueskivRenderer(UxContext, GameContext, game, AppConfig, gameConfig);
            var userInput = new RogueskivInputHandler(UxContext, game, renderer);
            var engine = new GameEngine<IEntity>(GameContext, userInput, game, renderer);

            FloorEngines.Add(engine);

            return engine;
        }

        private GameEngine<IEntity> CreateMenuStage()
        {
            StopCurrentFloorEngine();
            CurrentFloor = 1;
            FloorEngines.ForEach(gameEngine => gameEngine.Dispose());
            FloorEngines.Clear();
            UxContext.PlayMusic(AppConfig.MenuMusicFilePath, AppConfig.MenuMusicVolume);

            var gameContext = new GameContext(AppConfig.MaxGameStepsWithoutRender);
            var game = new RogueskivMenu(AppConfig, GameStageCodes.Menu);
            var renderer = new RogueskivMenuRenderer(UxContext, game, AppConfig.FontFile);
            var userInput = new RogueskivMenuInputHandler(UxContext, game, renderer);
            var engine = new GameEngine<IEntity>(gameContext, userInput, game, renderer);

            return engine;
        }

        private void StopCurrentFloorEngine()
        {
            if (FloorEngines.Any())
                FloorEngines[CurrentFloor - 1].Stop();
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
