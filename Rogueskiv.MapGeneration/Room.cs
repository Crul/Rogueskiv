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

        public bool Intersects(Room room) =>
            Intersects(room.TilePos, room.Size);

        public bool Intersects(Point tilePos, Size size) =>
            TilePos.X < (tilePos.X + size.Width)
                && (TilePos.X + Size.Width) > tilePos.X
                && TilePos.Y < (tilePos.Y + size.Height)
                && (TilePos.Y + Size.Height) > tilePos.Y;

        public bool IntersectsOrAdjacent(Room room) =>
            IntersectsOrAdjacent(room.TilePos, room.Size);

        public bool IntersectsOrAdjacent(Point tilePos, Size size) =>
            TilePos.X <= (tilePos.X + size.Width)
                && (TilePos.X + Size.Width) >= tilePos.X
                && TilePos.Y <= (tilePos.Y + size.Height)
                && (TilePos.Y + Size.Height) >= tilePos.Y;

        public bool HasMinSize(int minRoomSize) =>
            Size.Width >= minRoomSize && Size.Height >= minRoomSize;
    }
}
