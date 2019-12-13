namespace Rogueskiv.Engine
{
    public class GameContext : IGameContext
    {
        private const int TICKS_IN_A_SECOND = 10000000;

        public int GameFPS { get; }
        public long GameTicks { get; }
        public int UxFPS { get; }
        public long UxTicks { get; }

        public GameContext(int gameFPS = 25, int uxFPS = 60)
        {
            GameFPS = gameFPS;
            UxFPS = uxFPS;
            GameTicks = TICKS_IN_A_SECOND / gameFPS;
            UxTicks = TICKS_IN_A_SECOND / uxFPS;
        }
    }
}
