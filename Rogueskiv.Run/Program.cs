using Rogueskiv.Core;
using Rogueskiv.Engine;
using Rogueskiv.Ux;

namespace Rogueskiv.Run
{
    class Program
    {
        static void Main(string[] args)
        {
            var gameContext = new GameContext();
            var game = new RogueskivGame(gameContext);
            var userInput = new InputHandler<RogueskivGame>(game);
            var renderer = new RogueskivRenderer(game);

            var engine = new GameEngine(gameContext, userInput, game, renderer);

            engine.StartLoop();
        }
    }
}
