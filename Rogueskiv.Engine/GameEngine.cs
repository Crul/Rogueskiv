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
        private const int MAX_STEPS_WITHOUT_RENDER = 5;

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

        public void RunLoop()
        {
            var currentTime = CurrentTime();
            long nextGameTick = currentTime;
            long nextUxTick = currentTime;
            var stepsWithoutRender = 0;

            Game.Init();

            while (true)
            {
                InputHandler.ProcessEvents();
                while (ShouldUpdate(nextGameTick, stepsWithoutRender))
                {
                    Game.Update();
                    if (Game.Quit)
                        return;

                    nextGameTick += GameContext.GameTicks;
                    stepsWithoutRender++;
                }

                var nextActionTick = Math.Min(nextGameTick, nextUxTick);
                currentTime = CurrentTime();
                if (currentTime < nextActionTick)
                    Thread.Sleep((int)((nextActionTick - currentTime) / 10000));

                currentTime = CurrentTime();
                if (currentTime > nextUxTick)
                {
                    // TODO interpolate animations
                    var interpolation = 1f - (((float)(nextGameTick - currentTime)) / GameContext.GameTicks);
                    Renderer.Render(interpolation);
                    nextUxTick += GameContext.UxTicks;
                    stepsWithoutRender = 0;
                }
            }
        }

        private static bool ShouldUpdate(long nextGameTick, int stepsWithoutRender) =>
            CurrentTime() > nextGameTick && stepsWithoutRender < MAX_STEPS_WITHOUT_RENDER;

        private static long CurrentTime() => DateTime.Now.Ticks;
    }
}
