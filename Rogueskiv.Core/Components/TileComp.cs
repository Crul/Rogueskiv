namespace Rogueskiv.Core.Components
{
    public class TileComp : IComponent
    {
        private const int TILE_SIZE = 30;     // TODO proper tile size
        public int X { get; }
        public int Y { get; }

        public TileComp(int x, int y)
        {
            X = TILE_SIZE * x + (TILE_SIZE / 2);
            Y = TILE_SIZE * y + (TILE_SIZE / 2);
        }
    }
}
