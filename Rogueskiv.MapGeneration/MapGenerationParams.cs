using Seedwork.Crosscutting;
using System.Drawing;

namespace Rogueskiv.MapGeneration
{
    public class MapGenerationParams
    {
        public int Width { get; }
        public int Height { get; }
        public float MinDensity { get; }
        public int InitialRooms { get; }
        public int MinRoomSize { get; }

        private readonly float RoomExpandProbability;
        private readonly float CorridorTurnProbability;

        public MapGenerationParams(
            int width,
            int height,
            float roomExpandProbability,
            float corridorTurnProbability,
            float minDensity,
            int initialRooms,
            int minRoomSize
        )
        {
            Width = width;
            Height = height;
            RoomExpandProbability = roomExpandProbability;
            CorridorTurnProbability = corridorTurnProbability;
            MinDensity = minDensity;
            InitialRooms = initialRooms;
            MinRoomSize = minRoomSize;
        }

        public bool RoomExpandCheck() => Luck.NextDouble() < RoomExpandProbability;

        public bool CorridorTurnCheck() => Luck.NextDouble() < CorridorTurnProbability;

        public bool IsTileInBounds(Point tile) =>
            tile.X > 0
            && tile.X < Width - 1
            && tile.Y > 0
            && tile.Y < Height - 1;
    }
}
