using System.Drawing;

namespace Rogueskiv.Core.Components.Walls
{
    public class WallTile
    {
        public Point TilePos { get; }
        public WallTipTypes InitialTip { get; set; }
        public WallTipTypes FinalTip { get; set; }

        public WallTile(
            Point tilePos,
            WallTipTypes? initialTip = null,
            WallTipTypes? finalTip = null
        )
        {
            TilePos = tilePos;
            InitialTip = initialTip ?? WallTipTypes.FLAT;
            FinalTip = finalTip ?? WallTipTypes.FLAT;
        }
    }
}
