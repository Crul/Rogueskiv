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
        private readonly GameStages<EntityList> GameStages = new GameStages<EntityList>();
        private readonly List<GameEngine<EntityList>> FloorEngines = new List<GameEngine<EntityList>>();
        private readonly LoadingScreenRenderer LoadingScreenRenderer;

        private int CurrentFloor;
        private GameEngine<EntityList> CurrentFloorEngine => FloorEngines[CurrentFloor - 1];

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

        private GameEngine<EntityList> GetFloorUp(IGameResult<EntityList> result)
        {
            StopCurrentFloorEngine();

            return GetRestartFloorEngine(-1, result);
        }

        private GameEngine<EntityList> GetFloorDown(IGameResult<EntityList> result)
        {
            StopCurrentFloorEngine();
            if (CurrentFloor < FloorEngines.Count)
                return GetRestartFloorEngine(1, result);

            return CreateGameStage(result);
        }

        private GameEngine<EntityList> GetRestartFloorEngine(int floorMovement, IGameResult<EntityList> result)
        {
            var currentFloorEngine = CurrentFloorEngine;
            CurrentFloor += floorMovement;
            var nextFloorEngine = CurrentFloorEngine;
            nextFloorEngine.Game.Restart(result);
            nextFloorEngine.SetInputControls(currentFloorEngine.InputHandler);

            return nextFloorEngine;
        }

        private GameEngine<EntityList> CreateGameStage(
            IGameResult<EntityList> result = null,
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
            var game = new RogueskivGame(GameStageCodes.Game, gameConfig, CurrentFloor, result, OnGameEnd);
            var renderer = new RogueskivRenderer(UxContext, GameContext, game, AppConfig, gameConfig);
            var userInput = new RogueskivInputHandler(UxContext, game, renderer);
            var engine = new GameEngine<EntityList>(GameContext, userInput, game, renderer);

            FloorEngines.Add(engine);

            return engine;
        }

        private void OnGameEnd(RogueskivGameStats gameStats)
        {
            gameStats.GameMode = AppConfig.GameMode;
            gameStats.Floors = AppConfig.FloorCount;

            var yamlData = YamlParser.Serialize(new List<RogueskivGameStats> { gameStats });
            File.AppendAllText(AppConfig.GameStatsFilePath, yamlData);
        }

        private GameEngine<EntityList> CreateMenuStage()
        {
            StopCurrentFloorEngine();
            CurrentFloor = 1;
            FloorEngines.ForEach(gameEngine => gameEngine.Dispose());
            FloorEngines.Clear();
            UxContext.PlayMusic(AppConfig.MenuMusicFilePath, AppConfig.MenuMusicVolume);

            var gameContext = new GameContext(AppConfig.MaxGameStepsWithoutRender);
            var game = new RogueskivMenu(AppConfig, GameStageCodes.Menu, LoadStats);
            var renderer = new RogueskivMenuRenderer(UxContext, game, AppConfig.FontFile);
            var userInput = new RogueskivMenuInputHandler(UxContext, game, renderer);
            var engine = new GameEngine<EntityList>(gameContext, userInput, game, renderer);

            return engine;
        }

        private void StopCurrentFloorEngine()
        {
            if (FloorEngines.Any())
                CurrentFloorEngine.Stop();
        }

        private List<List<string>> LoadStats()
        {
            var data = YamlParser
                    .ParseFile<List<RogueskivGameStats>>(AppConfig.GameStatsFilePath);

            if (data == null)
                return new List<List<string>>();

            return data
                  .AsEnumerable()
                  .Reverse()
                  .Select(gameStat => new List<string>
                    {
                        gameStat.GetResult(),
                        gameStat.GetDateTime(),
                        RogueskivMenu.CleanGameModeText(gameStat.GameMode),
                        gameStat.Floors.ToString(),
                        gameStat.DiedOnFloor.ToString(),
                        gameStat.FinalHealth.ToString(),
                        gameStat.GetRealTimeFormatted(),
                        gameStat.GetInGameTimeFormatted(),
                  })
                  .ToList();
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
