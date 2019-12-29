using System.Drawing;

namespace Rogueskiv.MapGeneration
{
    public interface IMapGenerationParams
    {
        int Width { get; }
        int Height { get; }
        float MinDensity { get; }
        int InitialRooms { get; }
        int MinRoomSize { get; }
        int MinRoomSeparation { get; }

        bool RoomExpandCheck();
        bool CorridorTurnCheck();
        bool IsTileInBounds(Point tile);
        int GetRandomCorridorWidth();
    }
}
