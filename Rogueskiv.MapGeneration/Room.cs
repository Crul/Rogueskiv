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

        public bool HasTile(Point tile) =>
            Intersects(tile, new Size(1, 1));

        public bool Intersects(Room room, int margin = 0) =>
            Intersects(room.TilePos, room.Size, margin);

        public bool Intersects(Point tilePos, Size size, int margin = 0) =>
            TilePos.X < (tilePos.X + size.Width) + margin
                && (TilePos.X + Size.Width) > tilePos.X - margin
                && TilePos.Y < (tilePos.Y + size.Height) + margin
                && (TilePos.Y + Size.Height) > tilePos.Y - margin;

        public bool HasMinSize(int minRoomSize) =>
            Size.Width >= minRoomSize && Size.Height >= minRoomSize;
    }
}
