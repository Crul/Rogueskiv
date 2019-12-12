using Rogueskiv.Core;
using Rogueskiv.Engine;
using Rogueskiv.Ux;

namespace Rogueskiv.Run
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new RogueskivGame();
            var userInput = new InputHandler<RogueskivGame>(game);
            var renderer = new RogueskivRenderer(game);

            var engine = new GameEngine(userInput, game, renderer);

            engine.StartLoop();
        }
    }
}
