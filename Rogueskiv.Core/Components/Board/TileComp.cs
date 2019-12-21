using Rogueskiv.Core.Components.Position;
using System.Drawing;

namespace Rogueskiv.Core.Components.Board
{
    public class TileComp : PositionComp, IFOVComp
    {
        public bool HasBeenSeen { get; private set; }
        public bool VisibleByPlayer { get; private set; }
        public float DistanceFromPlayer { get; set; }

        public override bool Visible
        {
            get => HasBeenSeen;
            set
            {
                VisibleByPlayer = value;
                HasBeenSeen = HasBeenSeen || value;
            }
        }

        public TileComp(Point tilePos)
            : base(new PointF(
                BoardComp.TILE_SIZE * tilePos.X + (BoardComp.TILE_SIZE / 2),
                BoardComp.TILE_SIZE * tilePos.Y + (BoardComp.TILE_SIZE / 2)
            ))
        { }
    }
}
