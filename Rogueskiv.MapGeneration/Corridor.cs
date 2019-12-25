using System.Collections.Generic;
using System.Drawing;

namespace Rogueskiv.MapGeneration
{
    class Corridor
    {
        public Room StartRoom { get; set; }
        public Point StartTile { get; set; }
        public Room EndRoom { get; set; }
        public Point EndTile { get; set; }
        public List<Point> Tiles { get; set; }
        public Corridor() => Tiles = new List<Point>();
    }
}
