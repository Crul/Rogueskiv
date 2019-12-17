using Rogueskiv.Core;
using Rogueskiv.MapGeneration;
using Rogueskiv.Ux;
using Seedwork.Engine;
using Seedwork.Ux;
using System;

namespace Rogueskiv.Run
{
    class Program
    {
        private const int SCREEN_WIDTH = 800;
        private const int SCREEN_HEIGHT = 520;

        private readonly static GameStages GameStages = new GameStages();

        static void Main(string[] args)
        {
            using var uxContext = new UxContext("Rogueskiv", SCREEN_WIDTH, SCREEN_HEIGHT);
            var engine = GetInitialStage(uxContext);

            while (engine != null)
            {
                var gameResult = engine.RunLoop();
                engine = GameStages.GetNext(engine.Game.StageCode, gameResult);
            }
        }

        private static GameEngine GetInitialStage(UxContext uxContext)
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
            var game = new RogueskivGame(gameContext, boardData);
            var userInput = new RogueskivInputHandler(game);
            var renderer = new RogueskivRenderer(uxContext, game);
            var engine = new GameEngine(gameContext, userInput, game, renderer);

            return engine;
        }
    }
}
