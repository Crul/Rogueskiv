using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Rogueskiv.MapGeneration
{
    public static class MapGenerator
    {
        public static string GenerateMap(MapGenerationParams mapParams)
        {
            try
            {
                return TryGenerateMap(mapParams);
            }
            catch (InvalidMapException ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        private static string TryGenerateMap(MapGenerationParams mapParams)
        {
            var halfSizeMapParams = new MapGenerationParams(
                mapParams.Width / 2,
                mapParams.Height / 2,
                mapParams.RoomExpandProbability,
                mapParams.CorridorTurnProbability,
                mapParams.MinDensity,
                mapParams.InitialRooms,
                mapParams.MinRoomSize
            );

            var rooms = RoomGenerator.GenerateRooms(halfSizeMapParams);
            var corridors = CorridorGenerator.ConnectRooms(halfSizeMapParams, rooms);

            return PrintBoard(halfSizeMapParams, rooms, corridors);
        }

        private static string PrintBoard(
            MapGenerationParams mapParams, List<Room> rooms, List<Corridor> corridors
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
                        ? "TT" : corridors.Any(c => c.Tiles.Contains(tile)) ? "tt" : "..";
                }

                board += boardRow + Environment.NewLine;
                board += boardRow + Environment.NewLine;
            }

            return board;
        }
    }
}
