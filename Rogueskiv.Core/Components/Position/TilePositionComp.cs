using Rogueskiv.Core.Components.Board;
using Seedwork.Crosscutting;
using System.Drawing;

namespace Rogueskiv.Core.Components.Position
{
    public class TilePositionComp : PositionComp
    {
        public TilePositionComp(Point tilePos)
            : base(tilePos.Multiply(BoardComp.TILE_SIZE))
        { }
    }
}
