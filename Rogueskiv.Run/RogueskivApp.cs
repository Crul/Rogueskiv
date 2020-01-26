using Rogueskiv.Core;
using Rogueskiv.Menus;
using Rogueskiv.Menus.Renderers;
using Rogueskiv.Ux;
using Seedwork.Core.Entities;
using Seedwork.Crosscutting;
using Seedwork.Engine;
using Seedwork.Ux;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
        private GameEngine<IEntity> CurrentFloorEngine => FloorEngines[CurrentFloor - 1];

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
            SaveGlobalConfig();
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

            return GetRestartFloorEngine(-1, result);
        }

        private GameEngine<IEntity> GetFloorDown(IGameResult<IEntity> result)
        {
            StopCurrentFloorEngine();
            if (CurrentFloor < FloorEngines.Count)
                return GetRestartFloorEngine(1, result);

            return CreateGameStage(result);
        }

        private GameEngine<IEntity> GetRestartFloorEngine(int floorMovement, IGameResult<IEntity> result)
        {
            var currentFloorEngine = CurrentFloorEngine;
            CurrentFloor += floorMovement;
            var nextFloorEngine = CurrentFloorEngine;
            nextFloorEngine.Game.Restart(result);
            nextFloorEngine.SetInputControls(currentFloorEngine.InputHandler);

            return nextFloorEngine;
        }

        private GameEngine<IEntity> CreateGameStage(
            IGameResult<IEntity> result = null,
            int? gameSeed = null
        )
        {
            LoadingScreenRenderer.Render();

            var isFirstStage = result == null;
            if (isFirstStage)
                SaveGlobalConfig();

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
                CurrentFloorEngine.Stop();
        }

        private void SaveGlobalConfig()
        {
            var globalConfigText = File.ReadAllText(AppConfig.GlobalConfigFilePath);
            globalConfigText = ReplaceOptionValue(globalConfigText, "floorCount", AppConfig.FloorCount);
            globalConfigText = ReplaceOptionValue(globalConfigText, "gameModeIndex", AppConfig.GameModeIndex);
            globalConfigText = ReplaceOptionValue(globalConfigText, "musicOn", AppConfig.MusicOn.ToString().ToLower());

            File.WriteAllText(AppConfig.GlobalConfigFilePath, globalConfigText);
        }

        private static string ReplaceOptionValue(string globalConfigText, string optionName, object optionValue)
            => Regex.Replace(
                globalConfigText,
                "^" + optionName + @":\s*[a-z\d]+",
                $"{optionName}: {optionValue}",
                RegexOptions.Multiline | RegexOptions.IgnoreCase
            );

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
