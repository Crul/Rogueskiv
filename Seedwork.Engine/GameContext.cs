using Seedwork.Crosscutting;

namespace Seedwork.Engine
{
    public class GameContext : IGameContext
    {
        private const int TICKS_IN_A_SECOND = 10000000;

        public int GameSeed { get; private set; }
        public int GameFPS { get; }
        public long GameTicks { get; }
        public int UxFPS { get; }
        public long UxTicks { get; }
        public int MaxGameStepsWithoutRender { get; }

        public GameContext(int maxGameStepsWithoutRender, int gameFPS = 60, int uxFPS = 60)
        {
            GameSeed = Luck.Reset();
            GameFPS = gameFPS;
            UxFPS = uxFPS;
            GameTicks = TICKS_IN_A_SECOND / gameFPS;
            UxTicks = TICKS_IN_A_SECOND / uxFPS;
            MaxGameStepsWithoutRender = maxGameStepsWithoutRender;
        }

        public void SetSeed(int seed) => GameSeed = Luck.Reset(seed);
    }
}
