using Seedwork.Crosscutting;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Rogueskiv.MapGeneration
{
    public class MapGenerationParams : IMapGenerationParams
    {
        public int Width { get; }
        public int Height { get; }
        public float MinDensity { get; }
        public int InitialRooms { get; }
        public int MinRoomSize { get; }
        public int MinRoomSeparation { get; }

        private readonly float RoomExpandProbability;
        private readonly float CorridorTurnProbability;
        // TODO refactor XxxxxProbWeights
        private readonly List<(int width, float weight)> CorridorWidthProbWeights;

        public MapGenerationParams(
            int width,
            int height,
            float roomExpandProbability,
            float corridorTurnProbability,
            float minDensity,
            int initialRooms,
            int minRoomSize,
            int minRoomSeparation,
            List<(int width, float weight)> corridorWidthProbWeights
        )
        {
            Width = width;
            Height = height;
            RoomExpandProbability = roomExpandProbability;
            CorridorTurnProbability = corridorTurnProbability;
            MinDensity = minDensity;
            InitialRooms = initialRooms;
            MinRoomSize = minRoomSize;
            MinRoomSeparation = minRoomSeparation;
            CorridorWidthProbWeights = corridorWidthProbWeights;
        }

        public bool RoomExpandCheck() => Luck.NextDouble() < RoomExpandProbability;

        public bool CorridorTurnCheck() => Luck.NextDouble() < CorridorTurnProbability;

        public bool IsTileInBounds(Point tile) =>
            tile.X > 0
            && tile.X < Width - 1
            && tile.Y > 0
            && tile.Y < Height - 1;

        public int GetRandomCorridorWidth() =>
            CorridorWidthProbWeights
                .OrderByDescending(cwpw => cwpw.weight * Luck.NextDouble())
                .First()
                .width;
    }
}
