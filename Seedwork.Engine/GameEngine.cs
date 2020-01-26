using System;
using System.Threading;

namespace Seedwork.Engine
{
    public class GameEngine<T> : IDisposable
    {
        public IGame<T> Game { get; }
        public IInputHandler InputHandler { get; }

        private readonly IGameRenderer Renderer;
        private readonly IGameContext GameContext;

        public GameEngine(
            IGameContext gameContext,
            IInputHandler inputHandler,
            IGame<T> game,
            IGameRenderer renderer
        )
        {
            GameContext = gameContext;
            InputHandler = inputHandler;
            Game = game;
            Renderer = renderer;
        }

        public IGameResult<T> RunLoop()
        {
            Renderer.Reset();
            RunGameLoop();

            return Game.Result;
        }

        private void RunGameLoop()
        {
            var currentTime = CurrentTime();
            long nextGameTick = currentTime;
            long nextUxTick = currentTime;
            var stepsWithoutRender = 0;
            while (true)
            {
                while (ShouldUpdate(nextGameTick, stepsWithoutRender))
                {
                    InputHandler.ProcessEvents();
                    Game.Update();
                    if (Game.Quit)
                    {
                        Renderer.Render(0);
                        return;
                    }

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
                    var interpolation = 0f;
                    if (!Game.Pause)
                        interpolation = 1f - (((float)(nextGameTick - currentTime)) / GameContext.GameTicks);

                    Renderer.Render(interpolation);
                    nextUxTick += GameContext.UxTicks;
                    stepsWithoutRender = 0;
                }
            }
        }

        private bool ShouldUpdate(long nextGameTick, int gameStepsWithoutRender) =>
            CurrentTime() > nextGameTick && gameStepsWithoutRender < GameContext.MaxGameStepsWithoutRender;

        private static long CurrentTime() => DateTime.Now.Ticks;

        public void SetInputControls(IInputHandler inputHandler)
            => InputHandler.SetControls(inputHandler);

        public void Stop() => Renderer.Stop();

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                Renderer.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
