using Rogueskiv.Core;
using Rogueskiv.MapGeneration;
using Rogueskiv.Menus;
using Rogueskiv.Ux;
using Seedwork.Engine;
using Seedwork.Ux;
using System;
using System.Collections.Generic;

namespace Rogueskiv.Run
{
    class RogueskivApp
    {
        private const int SCREEN_WIDTH = 800;
        private const int SCREEN_HEIGHT = 520;

        private readonly GameStages GameStages = new GameStages();

        private readonly List<GameEngine> Floors = new List<GameEngine>();
        private int CurrentFloor;

        public void Run()
        {
            using var uxContext = new UxContext("Rogueskiv", SCREEN_WIDTH, SCREEN_HEIGHT);

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
                result => CreateGameStage(uxContext)
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

        private GameEngine CreateGameStage(UxContext uxContext)
        {
            // var boardData = File.ReadAllText(Path.Combine("data", "board.txt"));
            var boardData = MapGenerator.GenerateMap(
                    width: 64,
                    height: 32,
                    roomExpandProbability: 0.33f,
                    corridorTurnProbability: 0.1f,
                    minDensity: 0.33f,
                    initialRooms: 15,
                    minRoomSize: 3
                );

            Console.WriteLine(boardData);

            var gameContext = new GameContext();
            var game = new RogueskivGame(gameContext, boardData, GameStageCodes.Game);
            var userInput = new RogueskivInputHandler(game);
            var renderer = new RogueskivRenderer(uxContext, game);
            var engine = new GameEngine(gameContext, userInput, game, renderer);

            Floors.Add(engine);
            CurrentFloor = Floors.Count - 1;

            return engine;
        }

        private GameEngine CreateMenuStage(UxContext uxContext)
        {
            CurrentFloor = 0;

            var gameContext = new GameContext();
            var game = new RogueskivMenu(GameStageCodes.Menu);
            var userInput = new RogueskivMenuInputHandler(game);
            var renderer = new RogueskivMenuRenderer(uxContext, game);
            var engine = new GameEngine(gameContext, userInput, game, renderer);

            return engine;
        }
    }
}
