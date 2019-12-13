using System;
using System.Threading;

namespace Rogueskiv.Engine
{
    public class GameEngine
    {
        private readonly IGameContext GameContext;
        private readonly IInputHandler InputHandler;
        private readonly IGame Game;
        private readonly IRenderer Renderer;

        public GameEngine(
            IGameContext gameContext,
            IInputHandler inputHandler,
            IGame game,
            IRenderer renderer
        )
        {
            GameContext = gameContext;
            InputHandler = inputHandler;
            Game = game;
            Renderer = renderer;
        }

        public void StartLoop()
        {
            var currentTime = CurrentTime();
            RunLoop(currentTime, currentTime);
            Renderer.Dispose();
        }

        private void RunLoop(long nextGameTick, long nextUxTick)
        {
            while (true)
            {
                InputHandler.ProcessEvents();
                while (CurrentTime() > nextGameTick)
                {
                    Game.Update();
                    if (Game.Quit)
                        return;

                    nextGameTick += GameContext.GameTicks;
                }

                var nextActionTick = Math.Min(nextGameTick, nextUxTick);
                var currentTime = CurrentTime();
                if (currentTime < nextActionTick)
                    Thread.Sleep((int)((nextActionTick - currentTime) / 10000));

                if (CurrentTime() > nextUxTick)
                {
                    // TODO interpolate animations
                    Renderer.Render();
                    nextUxTick += GameContext.UxTicks;
                }
            }
        }

        private static long CurrentTime() => DateTime.Now.Ticks;
    }
}
