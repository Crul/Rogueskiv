using Seedwork.Crosscutting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.MapGeneration
{
    public static class MapGenerator
    {
        private const int INITIAL_ROOMS_MAX_LOOPS = 50;
        private const int EXPAND_CORRIDOR_MAX_LOOPS = 50;
        private const int CONNECT_ROOMS_MAX_LOOPS = 250;

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
            var area = mapParams.Width * mapParams.Height;
            var density = 0f;
            var rooms = new List<Room>();

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
                    .Where(room => RoomHasMinSize(room, mapParams.MinRoomSize))
                    .Sum(room => room.Width * room.Height);

                density = roomArea / area;
                if (density >= mapParams.MinDensity)
                    break;
            }

            rooms = rooms
                .Where(room => RoomHasMinSize(room, mapParams.MinRoomSize))
                .ToList();

            var corridors = ConnectRooms(mapParams, rooms);

            return PrintBoard(mapParams, rooms, corridors);
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

        private static List<Corridor> ConnectRooms(
            MapGenerationParams mapParams, List<Room> rooms
        )
        {
            var corridors = new List<Corridor>();
            var whileLoops = 0;
            while (true)
            {
                whileLoops++;
                if (whileLoops > CONNECT_ROOMS_MAX_LOOPS)
                    throw new InvalidMapException("Connecting rooms");

                var roomsWithoutCorridors = rooms.Where(r => r.Corridors.Count == 0).ToList();
                var room = (roomsWithoutCorridors.Count > 0)
                    ? roomsWithoutCorridors[Luck.Next(roomsWithoutCorridors.Count)]
                    : rooms[Luck.Next(rooms.Count)];

                var availableTiles = new List<((int x, int y) tile, Direction direction)>();
                for (var x = room.X + 1; x < room.X + room.Width - 1; x++)
                {
                    if (room.Y > 1)
                    {
                        var tileAbove = (tile: (x, room.Y - 1), dir: Direction.UP);
                        if (!room.Corridors.Any(c => c.Tiles.Contains(tileAbove.tile)))
                            availableTiles.Add(tileAbove);
                    }
                    if (room.Y + room.Height < mapParams.Height)
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
                    if (room.X + room.Width < mapParams.Width)
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

                ExpandCorridor(
                    mapParams, corridors, newCorridor, direction, rooms
                );

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
            MapGenerationParams mapParams,
            List<Corridor> corridors,
            Corridor newCorridor,
            Direction direction,
            List<Room> rooms
        )
        {
            var currentTile = (x: newCorridor.StartX, y: newCorridor.StartY);
            var whileLoops = 0;
            while (true)
            {
                whileLoops++;
                if (whileLoops > EXPAND_CORRIDOR_MAX_LOOPS)
                    throw new InvalidMapException("Expanding corridor");

                if (currentTile.x < 1
                    || currentTile.x > mapParams.Width - 2
                    || currentTile.y < 1
                    || currentTile.y > mapParams.Height - 2)
                    break;

                if (newCorridor.Tiles.Contains(currentTile)
                    || corridors.Any(c => c.Tiles.Contains(currentTile)))
                    break;

                newCorridor.Tiles.Add(currentTile);

                var endRooms = rooms.Where(room => RoomHasCell(room, currentTile.x, currentTile.y)).ToList();
                if (endRooms.Count > 0)
                {
                    if (endRooms.Count > 1)
                    {
                        Console.WriteLine(PrintBoard(mapParams, rooms, new List<Corridor>()));
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

                if (Luck.NextDouble() < mapParams.CorridorTurnProbability)
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
                if (rooms.Any(r => RoomHasCell(r, room.X - 2, y)))
                    return false;

            if (room.Y > 1 && rooms.Any(r => RoomHasCell(r, room.X - 1, room.Y - 1)))
                return false;

            if (room.Y + room.Height > height - 1 && rooms.Any(r => RoomHasCell(r, room.X - 1, room.Y + room.Height)))
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
                if (rooms.Any(r => RoomHasCell(r, room.X + room.Width + 1, y)))
                    return false;

            if (room.Y > 1 && rooms.Any(r => RoomHasCell(r, room.X + room.Width, room.Y - 1)))
                return false;

            if (room.Y + room.Height > mapParams.Height - 1
                && rooms.Any(r => RoomHasCell(r, room.X + room.Width, room.Y + room.Height)))
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
                if (rooms.Any(r => RoomHasCell(r, x, room.Y - 2)))
                    return false;

            if (room.X > 1 && rooms.Any(r => RoomHasCell(r, room.X - 1, room.Y - 1)))
                return false;

            if (room.X + room.Width > mapParams.Width - 1
                && rooms.Any(r => RoomHasCell(r, room.X + room.Width, room.Y - 1)))
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
                if (rooms.Any(r => RoomHasCell(r, x, room.Y + room.Height + 1)))
                    return false;

            if (room.X > 1 && rooms.Any(r => RoomHasCell(r, room.X - 1, room.Y + room.Height)))
                return false;

            if (room.X + room.Width > mapParams.Width - 1
                && rooms.Any(r => RoomHasCell(r, room.X + room.Width, room.Y + room.Height)))
                return false;

            return true;
        }

        private static string PrintBoard(
            MapGenerationParams mapParams, List<Room> rooms, List<Corridor> corridors
        )
        {
            var board = "";
            for (var y = 0; y < mapParams.Height; y++)
            {
                for (var x = 0; x < mapParams.Width; x++)
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
