using Rogueskiv.Core;
using Rogueskiv.MapGeneration;
using Rogueskiv.Ux;
using Seedwork.Engine;

namespace Rogueskiv.Run
{
    class Program
    {
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
            var game = new RogueskivGame(gameContext, boardData);
            var userInput = new RogueskivInputHandler(game);
            using var renderer = new RogueskivRenderer(game);
            var engine = new GameEngine(gameContext, userInput, game, renderer);

            engine.RunLoop();
        }
    }
}
