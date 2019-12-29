using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Rogueskiv.MapGeneration
{
    public static class MapGenerator
    {
        public static string GenerateMap(IMapGenerationParams mapParams)
        {
            try
            {
                return TryGenerateMap(mapParams);
            }
            catch (InvalidMapException)
            {
                return string.Empty;
            }
        }

        private static string TryGenerateMap(IMapGenerationParams mapParams)
        {
            var rooms = RoomGenerator.GenerateRooms(mapParams);
            var corridors = CorridorGenerator.ConnectRooms(mapParams, rooms);

            return PrintBoard(mapParams, rooms, corridors);
        }

        private static string PrintBoard(
            IMapGenerationParams mapParams, List<Room> rooms, List<Corridor> corridors
        )
        {
            var board = "";
            for (var y = 0; y < mapParams.Height; y++)
            {
                var boardRow = "";
                for (var x = 0; x < mapParams.Width; x++)
                {
                    var tile = new Point(x, y);
                    boardRow += rooms.Any(room => room.HasTile(tile))
                        ? "T"
                        : corridors.Any(c => c.Tiles.Contains(tile)) ? "t" : ".";
                }

                board += boardRow + Environment.NewLine;
            }

            return board;
        }
    }
}
