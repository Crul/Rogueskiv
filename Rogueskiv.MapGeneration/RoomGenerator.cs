using Seedwork.Crosscutting;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.MapGeneration
{
    class RoomGenerator
    {
        private const int INITIAL_ROOMS_MAX_LOOPS = 50;
        private const int MIN_ROOM_DISTANCE = 1;
        private const int MIN_ROOM_DISTANCE_PLUS_ONE = 1 + MIN_ROOM_DISTANCE;

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
                X = Luck.Next(1, mapParams.Width - 2),  // external wall border required
                Y = Luck.Next(1, mapParams.Height - 2), // external wall border required
                Width = 1,
                Height = 1,
                Expanded = false
            };

            var isAdjacentToOtherRoom = rooms.Any(room =>
                newRoom.X >= (room.X - MIN_ROOM_DISTANCE)
                && newRoom.X <= (room.X + room.Width + MIN_ROOM_DISTANCE)
                && newRoom.Y >= (room.Y - MIN_ROOM_DISTANCE)
                && newRoom.Y <= (room.Y + room.Height + MIN_ROOM_DISTANCE)
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
                && CanExpandLeft(rooms, room))
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
                && CanExpandUp(rooms, room))
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

        private static bool CanExpandLeft(List<Room> rooms, Room room)
        {
            if (room.X <= 2)
                return false;

            for (
                var y = room.Y - MIN_ROOM_DISTANCE_PLUS_ONE;
                y <= room.Y + room.Height + MIN_ROOM_DISTANCE_PLUS_ONE;
                y++
            )
                if (rooms.Any(r => Enumerable
                                    .Range(1, MIN_ROOM_DISTANCE_PLUS_ONE)
                                    .Any(i => r.HasTile(room.X - i, y))))
                    return false;

            return true;
        }

        private static bool CanExpandRight(
            MapGenerationParams mapParams, List<Room> rooms, Room room
        )
        {
            if (room.X + room.Width >= mapParams.Width - 2)
                return false;

            for (
                var y = room.Y - MIN_ROOM_DISTANCE_PLUS_ONE;
                y <= room.Y + room.Height + MIN_ROOM_DISTANCE_PLUS_ONE;
                y++
            )
                if (rooms.Any(r => Enumerable
                                    .Range(1, MIN_ROOM_DISTANCE_PLUS_ONE)
                                    .Any(i => r.HasTile(room.X + room.Width + i, y))))
                    return false;

            return true;
        }

        private static bool CanExpandUp(List<Room> rooms, Room room)
        {
            if (room.Y <= 1)
                return false;

            for (
                var x = room.X - MIN_ROOM_DISTANCE_PLUS_ONE;
                x <= room.X + room.Width + MIN_ROOM_DISTANCE_PLUS_ONE;
                x++
            )
                if (rooms.Any(r => Enumerable
                                    .Range(1, MIN_ROOM_DISTANCE_PLUS_ONE)
                                    .Any(i => r.HasTile(x, room.Y - i))))
                    return false;

            return true;
        }

        private static bool CanExpandDown(
            MapGenerationParams mapParams, List<Room> rooms, Room room
        )
        {
            if (room.Y + room.Height >= mapParams.Height - 2)
                return false;

            for (
                var x = room.X - MIN_ROOM_DISTANCE_PLUS_ONE;
                x <= room.X + room.Width + MIN_ROOM_DISTANCE_PLUS_ONE;
                x++
            )
                if (rooms.Any(r => Enumerable
                                    .Range(1, MIN_ROOM_DISTANCE_PLUS_ONE)
                                    .Any(i => r.HasTile(x, room.Y + room.Height + i))))
                    return false;

            return true;
        }
    }
}
