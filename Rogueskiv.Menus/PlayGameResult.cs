using Seedwork.Core.Entities;
using Seedwork.Engine;

namespace Rogueskiv.Menus
{
    public class PlayGameResult : GameResult<EntityList>
    {
        public int? GameSeed { get; set; } = null;

        public PlayGameResult(int resultCode) : base(resultCode) { }
    }
}
