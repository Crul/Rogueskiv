using Rogueskiv.Core.Components.Position;

namespace Rogueskiv.Core.Components.Board
{
    public class TileComp : PositionComp, IFOVComp
    {
        private const int TILE_SIZE = 30; // TODO proper tile size

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
            X = TILE_SIZE * x + (TILE_SIZE / 2);
            Y = TILE_SIZE * y + (TILE_SIZE / 2);
        }
    }
}
