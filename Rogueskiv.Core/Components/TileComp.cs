namespace Rogueskiv.Core.Components
{
    public class TileComp : PositionComp
    {
        private const int TILE_SIZE = 30; // TODO proper tile size

        public TileComp(int x, int y) : base()
        {
            X = TILE_SIZE * x + (TILE_SIZE / 2);
            Y = TILE_SIZE * y + (TILE_SIZE / 2);
        }
    }
}
