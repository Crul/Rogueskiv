using System.Collections.Generic;

namespace Rogueskiv.MapGeneration
{
    class Corridor
    {
        public Room StartRoom { get; set; }
        public int StartX { get; set; }
        public int StartY { get; set; }
        public Room EndRoom { get; set; }
        public int EndX { get; set; }
        public int EndY { get; set; }
        public List<(int x, int y)> Tiles { get; set; }
        public Corridor() => Tiles = new List<(int x, int y)>();
    }
}
