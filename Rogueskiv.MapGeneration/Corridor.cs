using System.Collections.Generic;
using System.Drawing;

namespace Rogueskiv.MapGeneration
{
    class Corridor
    {
        public List<Point> Tiles { get; set; }
        public List<Room> Rooms { get; set; }

        public Corridor()
        {
            Tiles = new List<Point>();
            Rooms = new List<Room>();
        }
    }
}
