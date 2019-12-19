namespace Rogueskiv.MapGeneration
{
    public class MapGenerationParams
    {
        public int Width { get; }
        public int Height { get; }
        public float RoomExpandProbability { get; }
        public float CorridorTurnProbability { get; }
        public float MinDensity { get; }
        public int InitialRooms { get; }
        public int MinRoomSize { get; }

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
    }
}
