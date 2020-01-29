using Seedwork.Core.Entities;
using Seedwork.Engine;

namespace Rogueskiv.Core
{
    public static class RogueskivGameResults
    {
        public static IGameResult<EntityList> WinResult { get; } = new GameResult<EntityList>(1);
        public static IGameResult<EntityList> DeathResult { get; } = new GameResult<EntityList>(2);
        public static IGameResult<EntityList> FloorDown { get; } = new GameResult<EntityList>(3);
        public static IGameResult<EntityList> FloorUp { get; } = new GameResult<EntityList>(4);
    }
}
