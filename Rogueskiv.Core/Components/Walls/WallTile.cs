namespace Rogueskiv.Core.Components.Walls
{
    public class WallTile
    {
        public (int x, int y) Position { get; }
        public WallTipTypes InitialTip { get; set; }
        public WallTipTypes FinalTip { get; set; }

        public WallTile(
            (int, int) position,
            WallTipTypes? initialTip = null,
            WallTipTypes? finalTip = null
        )
        {
            Position = position;
            InitialTip = initialTip ?? WallTipTypes.FLAT;
            FinalTip = finalTip ?? WallTipTypes.FLAT;
        }
    }
}
