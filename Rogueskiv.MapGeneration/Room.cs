using Seedwork.Crosscutting;
using System.Collections.Generic;
using System.Drawing;

namespace Rogueskiv.MapGeneration
{
    class Room
    {
        public Point TilePos { get; set; }
        public Size Size { get; set; }
        public bool Visited { get; set; }
        public List<Corridor> Corridors { get; set; }
        public int Area { get => Size.Width * Size.Height; }

        public Room() => Corridors = new List<Corridor>();

        public bool HasMinSize(int minRoomSize) =>
            Size.Width >= minRoomSize && Size.Height >= minRoomSize;

        public bool HasTile(Point tilePos) =>
            Intersects(TilePos, Size, tilePos, new Size(1, 1));

        public bool Intersects(Room room, int margin) =>
            Intersects(room.TilePos, room.Size, margin);

        public bool Intersects(Point tilePos, Size size, int margin) =>
            Intersects(
                TilePos.Add(x: -margin),
                Size.Add(width: 2 * margin),
                tilePos,
                size
            )
            || Intersects(
                TilePos.Add(y: -margin),
                Size.Add(height: 2 * margin),
                tilePos,
                size
            );

        private static bool Intersects(Point tilePos1, Size size1, Point tilePos2, Size size2) =>
            tilePos1.X < (tilePos2.X + size2.Width)
                && (tilePos1.X + size1.Width) > tilePos2.X
                && tilePos1.Y < (tilePos2.Y + size2.Height)
                && (tilePos1.Y + size1.Height) > tilePos2.Y;
    }
}
