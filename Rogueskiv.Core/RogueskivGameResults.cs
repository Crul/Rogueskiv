using Seedwork.Core.Entities;
using Seedwork.Engine;

namespace Rogueskiv.Core
{
    public static class RogueskivGameResults
    {
        public static IGameResult<IEntity> WinResult { get; } = new GameResult<IEntity>(1);
        public static IGameResult<IEntity> DeathResult { get; } = new GameResult<IEntity>(2);
        public static IGameResult<IEntity> FloorDown { get; } = new GameResult<IEntity>(3);
        public static IGameResult<IEntity> FloorUp { get; } = new GameResult<IEntity>(4);
    }
}
