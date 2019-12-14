using Rogueskiv.Core;
using Rogueskiv.Engine;
using Rogueskiv.Ux;
using System.IO;

namespace Rogueskiv.Run
{
    class Program
    {
        static void Main(string[] args)
        {
            var boardData = File.ReadAllText(Path.Combine("data", "board.txt"));

            var gameContext = new GameContext();
            var game = new RogueskivGame(gameContext, boardData);
            var userInput = new InputHandler<RogueskivGame>(game);
            using var renderer = new RogueskivRenderer(game);
            var engine = new GameEngine(gameContext, userInput, game, renderer);

            engine.RunLoop();
        }
    }
}
