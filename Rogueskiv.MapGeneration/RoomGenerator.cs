using Seedwork.Crosscutting;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.MapGeneration
{
    class RoomGenerator
    {
        private const int INITIAL_ROOMS_MAX_LOOPS = 50;

        public static List<Room> GenerateRooms(MapGenerationParams mapParams)
        {
            var rooms = new List<Room>();
            var area = mapParams.Width * mapParams.Height;
            var density = 0f;

            while (rooms.Count < mapParams.InitialRooms)
                AddNewRoom(mapParams, rooms);

            var whileLoops = 0;
            while (true)
            {
                whileLoops++;
                if (whileLoops > INITIAL_ROOMS_MAX_LOOPS)
                    throw new InvalidMapException("Creating initial rooms");

                rooms.ForEach(room => TryToExpand(mapParams, rooms, room));
                var expaned = rooms.Any(room => room.Expanded);
                if (!expaned)
                {
                    AddNewRoom(mapParams, rooms);
                    continue;
                }

                var roomArea = (float)rooms
                    .Where(room => room.HasMinSize(mapParams.MinRoomSize))
                    .Sum(room => room.Width * room.Height);

                density = roomArea / area;
                if (density >= mapParams.MinDensity)
                    break;
            }

            return rooms
                .Where(room => room.HasMinSize(mapParams.MinRoomSize))
                .ToList(); ;
        }

        private static void AddNewRoom(MapGenerationParams mapParams, List<Room> rooms)
        {
            var newRoom = new Room()
            {
                X = Luck.Next(1, mapParams.Width - 1),  // external wall border required
                Y = Luck.Next(1, mapParams.Height - 1), // external wall border required
                Width = 1,
                Height = 1,
                Expanded = false
            };

            var isAdjacentToOtherRoom = rooms.Any(room =>
                newRoom.X >= (room.X - 1)
                && newRoom.X <= (room.X + room.Width)
                && newRoom.Y >= (room.Y - 1)
                && newRoom.Y <= (room.Y + room.Height)
            );

            if (!isAdjacentToOtherRoom)
                rooms.Add(newRoom);
        }

        private static void TryToExpand(
            MapGenerationParams mapParams, List<Room> rooms, Room room
        )
        {
            room.Expanded = false;

            if (Luck.NextDouble() > mapParams.RoomExpandProbability
                && CanExpandLeft(mapParams.Height, rooms, room))
            {
                room.X -= 1;
                room.Width += 1;
                room.Expanded = true;
            }
            if (Luck.NextDouble() > mapParams.RoomExpandProbability
                && CanExpandRight(mapParams, rooms, room))
            {
                room.Width += 1;
                room.Expanded = true;
            }
            if (Luck.NextDouble() > mapParams.RoomExpandProbability
                && CanExpandUp(mapParams, rooms, room))
            {
                room.Y -= 1;
                room.Height += 1;
                room.Expanded = true;
            }
            if (Luck.NextDouble() > mapParams.RoomExpandProbability
                && CanExpandDown(mapParams, rooms, room))
            {
                room.Height += 1;
                room.Expanded = true;
            }
        }

        private static bool CanExpandLeft(int height, List<Room> rooms, Room room)
        {
            if (room.X <= 1)
                return false;

            for (var y = room.Y - 1; y < room.Y + room.Height + 1; y++)
                if (rooms.Any(r => r.HasTile(room.X - 2, y)))
                    return false;

            if (room.Y > 1 && rooms.Any(r => r.HasTile(room.X - 1, room.Y - 1)))
                return false;

            if (room.Y + room.Height > height - 1 && rooms.Any(r => r.HasTile(room.X - 1, room.Y + room.Height)))
                return false;

            return true;
        }

        private static bool CanExpandRight(
            MapGenerationParams mapParams, List<Room> rooms, Room room
        )
        {
            if (room.X + room.Width >= mapParams.Width - 2)
                return false;

            for (var y = room.Y - 1; y < room.Y + room.Height + 1; y++)
                if (rooms.Any(r => r.HasTile(room.X + room.Width + 1, y)))
                    return false;

            if (room.Y > 1 && rooms.Any(r => r.HasTile(room.X + room.Width, room.Y - 1)))
                return false;

            if (room.Y + room.Height > mapParams.Height - 1
                && rooms.Any(r => r.HasTile(room.X + room.Width, room.Y + room.Height)))
                return false;

            return true;
        }

        private static bool CanExpandUp(
            MapGenerationParams mapParams, List<Room> rooms, Room room
        )
        {
            if (room.Y <= 1)
                return false;

            for (var x = room.X - 1; x < room.X + room.Width + 1; x++)
                if (rooms.Any(r => r.HasTile(x, room.Y - 2)))
                    return false;

            if (room.X > 1 && rooms.Any(r => r.HasTile(room.X - 1, room.Y - 1)))
                return false;

            if (room.X + room.Width > mapParams.Width - 1
                && rooms.Any(r => r.HasTile(room.X + room.Width, room.Y - 1)))
                return false;

            return true;
        }

        private static bool CanExpandDown(
            MapGenerationParams mapParams, List<Room> rooms, Room room
        )
        {
            if (room.Y + room.Height >= mapParams.Height - 2)
                return false;

            for (var x = room.X - 1; x < room.X + room.Width + 1; x++)
                if (rooms.Any(r => r.HasTile(x, room.Y + room.Height + 1)))
                    return false;

            if (room.X > 1 && rooms.Any(r => r.HasTile(room.X - 1, room.Y + room.Height)))
                return false;

            if (room.X + room.Width > mapParams.Width - 1
                && rooms.Any(r => r.HasTile(room.X + room.Width, room.Y + room.Height)))
                return false;

            return true;
        }
    }
}
