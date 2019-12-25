using Seedwork.Crosscutting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.MapGeneration
{
    class CorridorGenerator
    {
        private const int CONNECT_ROOMS_MAX_LOOPS = 250;
        private const int EXPAND_CORRIDOR_MAX_LOOPS = 50;

        public static List<Corridor> ConnectRooms(
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

                var room = rooms.OrderBy(r => r.Corridors.Count).First();

                var availableTiles = new List<((int x, int y) tile, Direction direction)>();
                for (var x = room.TilePos.X + 1; x < room.TilePos.X + room.Size.Width - 1; x++)
                {
                    if (room.TilePos.Y > 1)
                    {
                        var tileAbove = (tile: (x, room.TilePos.Y - 1), dir: Direction.UP);
                        if (!room.Corridors.Any(c => c.Tiles.Contains(tileAbove.tile)))
                            availableTiles.Add(tileAbove);
                    }
                    if (room.TilePos.Y + room.Size.Height < mapParams.Height)
                    {
                        var tileBelow = (tile: (x, room.TilePos.Y + room.Size.Height), Direction.DOWN);
                        if (!room.Corridors.Any(c => c.Tiles.Contains(tileBelow.tile)))
                            availableTiles.Add(tileBelow);
                    }
                }

                for (var y = room.TilePos.Y; y < room.TilePos.Y + room.Size.Height; y++)
                {
                    if (room.TilePos.X > 1)
                    {
                        var tileLeft = (tile: (room.TilePos.X - 1, y), dir: Direction.LEFT);
                        if (!room.Corridors.Any(c => c.Tiles.Contains(tileLeft.tile)))
                            availableTiles.Add(tileLeft);
                    }
                    if (room.TilePos.X + room.Size.Width < mapParams.Width)
                    {
                        var tileRight = (tile: (room.TilePos.X + room.Size.Width, y), dir: Direction.RIGHT);
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

                var endRooms = rooms.Where(room => room.HasTile(currentTile.x, currentTile.y)).ToList();
                if (endRooms.Count > 0)
                {
                    if (endRooms.Count > 1)
                        throw new Exception("More than 1 room with the same tile");

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
    }
}
