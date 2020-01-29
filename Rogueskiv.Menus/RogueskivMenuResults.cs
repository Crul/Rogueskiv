using Seedwork.Core.Entities;
using Seedwork.Engine;

namespace Rogueskiv.Menus
{
    public static class RogueskivMenuResults
    {
        public static PlayGameResult PlayResult { get; } = new PlayGameResult(1);
        public static IGameResult<EntityList> QuitResult { get; } = new GameResult<EntityList>(2);
    }
}
