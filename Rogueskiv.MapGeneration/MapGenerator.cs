using Seedwork.Crosscutting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.MapGeneration
{
    public static class MapGenerator
    {
        public static string GenerateMap(
            int width,
            int height,
            float roomExpandProbability,
            float corridorTurnProbability,
            float minDensity,
            int initialRooms,
            int minRoomSize
        )
        {
            var area = width * height;
            var density = 0f;
            var rooms = new List<Room>();

            while (rooms.Count < initialRooms)
                AddNewRoom(width, height, rooms);

            while (true)
            {
                rooms.ForEach(room => TryToExpand(width, height, rooms, room, roomExpandProbability));
                var expaned = rooms.Any(room => room.Expanded);
                if (!expaned)
                {
                    AddNewRoom(width, height, rooms);
                    continue;
                }

                var roomArea = (float)rooms
                    .Where(room => RoomHasMinSize(room, minRoomSize))
                    .Sum(room => room.Width * room.Height);

                density = roomArea / area;
                if (density >= minDensity)
                    break;
            }

            rooms = rooms
                .Where(room => RoomHasMinSize(room, minRoomSize))
                .ToList();

            var corridors = ConnectRooms(width, height, rooms, corridorTurnProbability);

            return PrintBoard(width, height, rooms, corridors);
        }

        private static void AddNewRoom(int width, int height, List<Room> rooms)
        {
            var newRoom = new Room()
            {
                X = Luck.Next(1, width - 1),  // external wall border required
                Y = Luck.Next(1, height - 1), // external wall border required
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

        private static List<Corridor> ConnectRooms(
            int width, int height, List<Room> rooms, float turnProbability
        )
        {
            var corridors = new List<Corridor>();
            while (true)
            {
                var room = rooms[Luck.Next(rooms.Count)];

                var availableTiles = new List<((int x, int y) tile, Direction direction)>();
                for (var x = room.X + 1; x < room.X + room.Width - 1; x++)
                {
                    if (room.Y > 1)
                    {
                        var tileAbove = (tile: (x, room.Y - 1), dir: Direction.UP);
                        if (!room.Corridors.Any(c => c.Tiles.Contains(tileAbove.tile)))
                            availableTiles.Add(tileAbove);
                    }
                    if (room.Y + room.Height < height)
                    {
                        var tileBelow = (tile: (x, room.Y + room.Height), Direction.DOWN);
                        if (!room.Corridors.Any(c => c.Tiles.Contains(tileBelow.tile)))
                            availableTiles.Add(tileBelow);
                    }
                }

                for (var y = room.Y; y < room.Y + room.Height; y++)
                {
                    if (room.X > 1)
                    {
                        var tileLeft = (tile: (room.X - 1, y), dir: Direction.LEFT);
                        if (!room.Corridors.Any(c => c.Tiles.Contains(tileLeft.tile)))
                            availableTiles.Add(tileLeft);
                    }
                    if (room.X + room.Width < width)
                    {
                        var tileRight = (tile: (room.X + room.Width, y), dir: Direction.RIGHT);
                        if (!room.Corridors.Any(c => c.Tiles.Contains(tileRight.tile)))
                            availableTiles.Add(tileRight);
                    }
                }

                if (availableTiles.Count == 0)
                    continue;

                var newCorridor = new Corridor();

                var (tile, direction) = availableTiles[Luck.Next(availableTiles.Count)];

                newCorridor.StartRoom = room;
                newCorridor.StartX = tile.x;
                newCorridor.StartY = tile.y;

                ExpandCorridor(width, height, newCorridor, direction, rooms, turnProbability);

                if (newCorridor.EndRoom == null)
                    continue;

                newCorridor.StartRoom.Corridors.Add(newCorridor);
                newCorridor.EndRoom.Corridors.Add(newCorridor);
                corridors.Add(newCorridor);

                if (AreAllConnected(rooms))
                    break;
            }

            return corridors;
        }

        private static void ExpandCorridor(
            int width, int height, Corridor newCorridor, Direction direction, List<Room> rooms, float turnProbability
        )
        {
            var currentTile = (x: newCorridor.StartX, y: newCorridor.StartY);
            while (true)
            {
                if (currentTile.x < 1 || currentTile.x > width - 2 || currentTile.y < 1 || currentTile.y > height - 2)
                    break;

                if (newCorridor.Tiles.Contains(currentTile))
                    break;

                newCorridor.Tiles.Add(currentTile);

                var endRooms = rooms.Where(room => RoomHasCell(room, currentTile.x, currentTile.y)).ToList();
                if (endRooms.Count > 0)
                {
                    if (endRooms.Count > 1)
                    {
                        Console.WriteLine(PrintBoard(width, height, rooms, new List<Corridor>()));
                        throw new Exception("More than 1 room with the same tile");
                    }
                    var endRoom = endRooms.Single();
                    newCorridor.EndRoom = endRoom;
                    newCorridor.EndX = newCorridor.Tiles.Last().x;
                    newCorridor.EndY = newCorridor.Tiles.Last().y;
                    break;
                }

                switch (direction)
                {
                    case Direction.UP:
                        currentTile.y -= 1;
                        break;
                    case Direction.RIGHT:
                        currentTile.x += 1;
                        break;
                    case Direction.DOWN:
                        currentTile.y += 1;
                        break;
                    case Direction.LEFT:
                        currentTile.x -= 1;
                        break;
                }

                if (Luck.NextDouble() < turnProbability)
                {
                    if (direction == Direction.UP || direction == Direction.DOWN)
                        direction = (Luck.NextDouble() > 0.5 ? Direction.LEFT : Direction.RIGHT);
                    else
                        direction = (Luck.NextDouble() > 0.5 ? Direction.UP : Direction.DOWN);
                }
            }
        }

        private static bool AreAllConnected(List<Room> rooms)
        {
            rooms.ForEach(room => room.Visited = false);

            VisitRoom(rooms[0]);

            return rooms.All(room => room.Visited);
            /*
            while (true)
            {
                var connectedRooms = currentRoom
                    .Corridors
                    .SelectMany(corridor => new List<Room> { corridor.StartRoom, corridor.EndRoom })
                    .Distinct()
                    .Where(room => room != currentRoom)
                    .ToList();

                if (connectedRooms.Count == 0)
                    return false;

                rooms.Where(room => connectedRooms.Contains(info))
                    .ToList()
                    .ForEach(info => info.visited = true);

                var nonVisited = data.Where(info => !info.visited).ToList();
                if (nonVisited.Count == 0)
                    return true;

                currentRoom =
            }
            */
        }

        private static void VisitRoom(Room room)
        {
            room.Visited = true;
            room.Corridors
                .SelectMany(corridor => new List<Room> { corridor.StartRoom, corridor.EndRoom })
                .Distinct()
                .Where(room => !room.Visited)
                .ToList()
                .ForEach(VisitRoom);
        }

        private static void TryToExpand(
            int width, int height, List<Room> rooms, Room room, float expandProbability
        )
        {
            room.Expanded = false;

            if (Luck.NextDouble() > expandProbability && CanExpandLeft(height, rooms, room))
            {
                room.X -= 1;
                room.Width += 1;
                room.Expanded = true;
            }
            if (Luck.NextDouble() > expandProbability && CanExpandRight(width, height, rooms, room))
            {
                room.Width += 1;
                room.Expanded = true;
            }
            if (Luck.NextDouble() > expandProbability && CanExpandUp(width, rooms, room))
            {
                room.Y -= 1;
                room.Height += 1;
                room.Expanded = true;
            }
            if (Luck.NextDouble() > expandProbability && CanExpandDown(width, height, rooms, room))
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
                if (rooms.Any(r => RoomHasCell(r, room.X - 2, y)))
                    return false;

            if (room.Y > 1 && rooms.Any(r => RoomHasCell(r, room.X - 1, room.Y - 1)))
                return false;

            if (room.Y + room.Height > height - 1 && rooms.Any(r => RoomHasCell(r, room.X - 1, room.Y + room.Height)))
                return false;

            return true;
        }

        private static bool CanExpandRight(int width, int height, List<Room> rooms, Room room)
        {
            if (room.X + room.Width >= width - 2)
                return false;

            for (var y = room.Y - 1; y < room.Y + room.Height + 1; y++)
                if (rooms.Any(r => RoomHasCell(r, room.X + room.Width + 1, y)))
                    return false;

            if (room.Y > 1 && rooms.Any(r => RoomHasCell(r, room.X + room.Width, room.Y - 1)))
                return false;

            if (room.Y + room.Height > height - 1 && rooms.Any(r => RoomHasCell(r, room.X + room.Width, room.Y + room.Height)))
                return false;

            return true;
        }

        private static bool CanExpandUp(int width, List<Room> rooms, Room room)
        {
            if (room.Y <= 1)
                return false;

            for (var x = room.X - 1; x < room.X + room.Width + 1; x++)
                if (rooms.Any(r => RoomHasCell(r, x, room.Y - 2)))
                    return false;

            if (room.X > 1 && rooms.Any(r => RoomHasCell(r, room.X - 1, room.Y - 1)))
                return false;

            if (room.X + room.Width > width - 1 && rooms.Any(r => RoomHasCell(r, room.X + room.Width, room.Y - 1)))
                return false;

            return true;
        }

        private static bool CanExpandDown(int width, int height, List<Room> rooms, Room room)
        {
            if (room.Y + room.Height >= height - 2)
                return false;

            for (var x = room.X - 1; x < room.X + room.Width + 1; x++)
                if (rooms.Any(r => RoomHasCell(r, x, room.Y + room.Height + 1)))
                    return false;

            if (room.X > 1 && rooms.Any(r => RoomHasCell(r, room.X - 1, room.Y + room.Height)))
                return false;

            if (room.X + room.Width > width - 1 && rooms.Any(r => RoomHasCell(r, room.X + room.Width, room.Y + room.Height)))
                return false;

            return true;
        }

        private static string PrintBoard(int width, int height, List<Room> rooms, List<Corridor> corridors)
        {
            var board = "";
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                    board += rooms.Any(room => RoomHasCell(room, x, y))
                        ? "T" : corridors.Any(c => c.Tiles.Contains((x, y))) ? "T" : ".";

                board += Environment.NewLine;
            }

            return board;
        }

        private static bool RoomHasMinSize(Room room, int minRoomSize) =>
            room.Width >= minRoomSize && room.Height >= minRoomSize;

        private static bool RoomHasCell
            (Room room, int cellX, int cellY) =>
            cellX >= room.X && cellX < (room.X + room.Width)
            && cellY >= room.Y && cellY < (room.Y + room.Height);
    }
}
