using Seedwork.Core.Entities;
using Seedwork.Engine;

namespace Rogueskiv.Menus
{
    public static class RogueskivMenuResults
    {
        public static IGameResult<IEntity> PlayResult { get; } = new GameResult<IEntity>(1);
        public static IGameResult<IEntity> QuitResult { get; } = new GameResult<IEntity>(2);
    }
}
