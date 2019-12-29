using Seedwork.Crosscutting;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Rogueskiv.MapGeneration
{
    class RoomGenerator
    {
        private const int MIN_ROOM_SEPARATION = 2;
        private const int INITIAL_ROOMS_MAX_LOOPS = 150;

        public static List<Room> GenerateRooms(IMapGenerationParams mapParams)
        {
            var rooms = new List<Room>();
            var area = mapParams.Width * mapParams.Height;

            while (rooms.Count < mapParams.InitialRooms)
                AddNewRoom(mapParams, rooms);

            var whileLoops = 0;
            while (true)
            {
                whileLoops++;
                if (whileLoops > INITIAL_ROOMS_MAX_LOOPS)
                    throw new InvalidMapException("Creating initial rooms");

                foreach (var room in rooms)
                {
                    var expanded = TryToExpand(mapParams, rooms, room);
                    if (expanded)
                    {
                        var roomsWithMinSize = rooms
                            .Where(room => room.HasMinSize(mapParams.MinRoomSize))
                            .ToList();

                        var roomArea = (float)roomsWithMinSize.Sum(room => room.Area);
                        var density = roomArea / area;
                        if (density >= mapParams.MinDensity)
                            return roomsWithMinSize;
                    }
                }
            }
        }

        private static void AddNewRoom(IMapGenerationParams mapParams, List<Room> rooms)
        {
            var newRoom = new Room()
            {
                TilePos = new Point(
                    x: Luck.Next(1, mapParams.Width - 1),  // external wall border required
                    y: Luck.Next(1, mapParams.Height - 1)  // external wall border required
                ),
                Size = new Size(1, 1)
            };

            var isNotAdjacentToOtherRooms = rooms
                .All(room => !room.Intersects(newRoom, MIN_ROOM_SEPARATION));

            if (isNotAdjacentToOtherRooms)
                rooms.Add(newRoom);
        }

        private static bool TryToExpand(
            IMapGenerationParams mapParams, List<Room> rooms, Room room
        )
        {
            var expanded = false;
            if (mapParams.RoomExpandCheck() && CanExpandLeft(rooms, room))
            {
                room.TilePos = room.TilePos.Substract(x: 1);
                room.Size = room.Size.Add(width: 1);
                expanded = true;
            }
            if (mapParams.RoomExpandCheck() && CanExpandRight(mapParams, rooms, room))
            {
                room.Size = room.Size.Add(width: 1);
                expanded = true;
            }
            if (mapParams.RoomExpandCheck() && CanExpandUp(rooms, room))
            {
                room.TilePos = room.TilePos.Substract(y: 1);
                room.Size = room.Size.Add(height: 1);
                expanded = true;
            }
            if (mapParams.RoomExpandCheck() && CanExpandDown(mapParams, rooms, room))
            {
                room.Size = room.Size.Add(height: 1);
                expanded = true;
            }

            return expanded;
        }

        private static bool CanExpandLeft(List<Room> rooms, Room room)
        {
            var targetX = room.TilePos.X - 1;
            if (targetX <= 0)
                return false;

            return CanExpandHorizontally(rooms, room, targetX);
        }

        private static bool CanExpandRight(
            IMapGenerationParams mapParams, List<Room> rooms, Room room
        )
        {
            var targetX = room.TilePos.X + room.Size.Width;
            if (targetX >= mapParams.Width - 1)
                return false;

            return CanExpandHorizontally(rooms, room, targetX);
        }

        private static bool CanExpandUp(List<Room> rooms, Room room)
        {
            var targetY = room.TilePos.Y - 1;
            if (targetY <= 0)
                return false;

            return CanExpandVertically(rooms, room, targetY);
        }

        private static bool CanExpandDown(
            IMapGenerationParams mapParams, List<Room> rooms, Room room
        )
        {
            var targetY = room.TilePos.Y + room.Size.Height;
            if (targetY >= mapParams.Height - 1)
                return false;

            return CanExpandVertically(rooms, room, targetY);
        }

        private static bool CanExpandHorizontally(
            List<Room> rooms, Room room, int targetX
        )
        {
            var fromY = room.TilePos.Y;
            var toY = room.TilePos.Y + room.Size.Height;

            return rooms
                .Where(r => r != room)
                .All(r => !r.Intersects(
                    new Point(targetX, fromY),
                    new Size(1, toY - fromY),
                    MIN_ROOM_SEPARATION
                ));
        }

        private static bool CanExpandVertically(
            List<Room> rooms, Room room, int targetY
        )
        {
            var fromX = room.TilePos.X;
            var toX = room.TilePos.X + room.Size.Width;

            return rooms
                .Where(r => r != room)
                .All(r => !r.Intersects(
                    new Point(fromX, targetY),
                    new Size(toX - fromX, 1),
                    MIN_ROOM_SEPARATION
                ));
        }
    }
}
