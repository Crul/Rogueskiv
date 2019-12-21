using Rogueskiv.Core.Components.Position;

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

        public TileComp(int x, int y) : base()
        {
            X = BoardComp.TILE_SIZE * x + (BoardComp.TILE_SIZE / 2);
            Y = BoardComp.TILE_SIZE * y + (BoardComp.TILE_SIZE / 2);
        }
    }
}
