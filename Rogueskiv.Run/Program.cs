using Rogueskiv.Core;
using Rogueskiv.MapGeneration;
using Rogueskiv.Ux;
using Seedwork.Engine;
using Seedwork.Ux;

namespace Rogueskiv.Run
{
    class Program
    {
        private const int SCREEN_WIDTH = 800;
        private const int SCREEN_HEIGHT = 520;

        static void Main(string[] args)
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

            System.Console.WriteLine(boardData);

            var gameContext = new GameContext();
            var uxContext = new UxContext("Rogueskiv", SCREEN_WIDTH, SCREEN_HEIGHT);
            var game = new RogueskivGame(gameContext, boardData);
            var userInput = new RogueskivInputHandler(game);
            using var renderer = new RogueskivRenderer(uxContext, game);

            var engine = new GameEngine(gameContext, userInput, game, renderer);

            engine.RunLoop();
        }
    }
}
