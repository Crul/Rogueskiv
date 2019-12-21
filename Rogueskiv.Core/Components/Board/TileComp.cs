using Rogueskiv.Core.Components.Position;
using Seedwork.Crosscutting;
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
            : base(tilePos.Multiply(BoardComp.TILE_SIZE).Add(BoardComp.TILE_SIZE / 2)) { }
    }
}
