using Seedwork.Engine;

namespace Rogueskiv.Menus
{
    public static class RogueskivMenuResults
    {
        public static IGameResult PlayResult { get; } = new GameResult(1);
        public static IGameResult QuitResult { get; } = new GameResult(2);
    }
}
