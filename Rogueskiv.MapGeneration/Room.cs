﻿using System.Collections.Generic;

namespace Rogueskiv.MapGeneration
{
    class Room
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool Expanded { get; set; }
        public bool Visited { get; set; }
        public List<Corridor> Corridors { get; set; }
        public Room() => Corridors = new List<Corridor>();

        public bool HasTile(int tileX, int tileY) =>
            tileX >= X && tileX < (X + Width)
            && tileY >= Y && tileY < (Y + Height);

        public bool HasMinSize(int minRoomSize) =>
            Width >= minRoomSize && Height >= minRoomSize;
    }
}
