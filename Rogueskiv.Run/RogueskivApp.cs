using Rogueskiv.Core;
using Rogueskiv.Menus;
using Rogueskiv.Ux;
using Seedwork.Engine;
using Seedwork.Ux;
using System.Collections.Generic;

namespace Rogueskiv.Run
{
    class RogueskivApp
    {
        private const int SCREEN_WIDTH = 800;
        private const int SCREEN_HEIGHT = 520;
        private const int FLOOR_COUNT = 10;

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
                result => GetFloorDown(uxContext)
            );
            GameStages.Add(
                (GameStageCodes.Game, RogueskivGameResults.FloorUp.ResultCode),
                result => GetFloorUp()
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

        private GameEngine GetFloorUp() => Floors[--CurrentFloor];

        private GameEngine GetFloorDown(UxContext uxContext)
        {
            if (CurrentFloor + 1 < Floors.Count)
                return Floors[++CurrentFloor];

            return CreateGameStage(uxContext);
        }

        private GameEngine CreateGameStage(UxContext uxContext)
        {
            // var boardData = File.ReadAllText(Path.Combine("data", "board.txt"));

            CurrentFloor = Floors.Count;

            var gameContext = new GameContext();
            var game = new RogueskivGame(
                gameContext, GameStageCodes.Game, FLOOR_COUNT, CurrentFloor
            );
            var userInput = new RogueskivInputHandler(game);
            var renderer = new RogueskivRenderer(uxContext, game);
            var engine = new GameEngine(gameContext, userInput, game, renderer);

            Floors.Add(engine);

            return engine;
        }

        private GameEngine CreateMenuStage(UxContext uxContext)
        {
            CurrentFloor = 0;
            Floors.ForEach(gameEngine => gameEngine.Dispose());
            Floors.Clear();

            var gameContext = new GameContext();
            var game = new RogueskivMenu(GameStageCodes.Menu);
            var userInput = new RogueskivMenuInputHandler(game);
            var renderer = new RogueskivMenuRenderer(uxContext, game);
            var engine = new GameEngine(gameContext, userInput, game, renderer);

            return engine;
        }
    }
}
