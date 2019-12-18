using Seedwork.Engine;

namespace Rogueskiv.Core
{
    public static class RogueskivGameResults
    {
        public static IGameResult WinResult { get; } = new GameResult(1);
        public static IGameResult DeathResult { get; } = new GameResult(2);
        public static IGameResult FloorDown { get; } = new GameResult(3);
        public static IGameResult FloorUp { get; } = new GameResult(4);
    }
}
