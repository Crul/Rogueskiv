using Seedwork.Crosscutting;
using System;
using System.Collections.Generic;
using System.Drawing;
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

                var availableTiles = new List<(Point tile, Direction direction)>();
                for (var x = room.TilePos.X + 1; x < room.TilePos.X + room.Size.Width - 1; x++)
                {
                    if (room.TilePos.Y > 1)
                    {
                        var tileAbove = (tile: new Point(x, room.TilePos.Y - 1), dir: Direction.UP);
                        if (!room.Corridors.Any(c => c.Tiles.Contains(tileAbove.tile)))
                            availableTiles.Add(tileAbove);
                    }
                    if (room.TilePos.Y + room.Size.Height < mapParams.Height)
                    {
                        var tileBelow = (tile: new Point(x, room.TilePos.Y + room.Size.Height), Direction.DOWN);
                        if (!room.Corridors.Any(c => c.Tiles.Contains(tileBelow.tile)))
                            availableTiles.Add(tileBelow);
                    }
                }

                for (var y = room.TilePos.Y; y < room.TilePos.Y + room.Size.Height; y++)
                {
                    if (room.TilePos.X > 1)
                    {
                        var tileLeft = (tile: new Point(room.TilePos.X - 1, y), dir: Direction.LEFT);
                        if (!room.Corridors.Any(c => c.Tiles.Contains(tileLeft.tile)))
                            availableTiles.Add(tileLeft);
                    }
                    if (room.TilePos.X + room.Size.Width < mapParams.Width)
                    {
                        var tileRight = (tile: new Point(room.TilePos.X + room.Size.Width, y), dir: Direction.RIGHT);
                        if (!room.Corridors.Any(c => c.Tiles.Contains(tileRight.tile)))
                            availableTiles.Add(tileRight);
                    }
                }

                if (availableTiles.Count == 0)
                    continue;

                var newCorridor = new Corridor();

                var (tile, direction) = availableTiles[Luck.Next(availableTiles.Count)];

                newCorridor.StartRoom = room;
                newCorridor.StartTile = tile;

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
            var currentTile = newCorridor.StartTile;
            var whileLoops = 0;
            while (true)
            {
                whileLoops++;
                if (whileLoops > EXPAND_CORRIDOR_MAX_LOOPS)
                    throw new InvalidMapException("Expanding corridor");

                if (currentTile.X < 1
                    || currentTile.X > mapParams.Width - 2
                    || currentTile.Y < 1
                    || currentTile.Y > mapParams.Height - 2)
                    break;

                if (newCorridor.Tiles.Contains(currentTile)
                    || corridors.Any(c => c.Tiles.Contains(currentTile)))
                    break;

                newCorridor.Tiles.Add(currentTile);

                var endRooms = rooms.Where(room => room.HasTile(currentTile)).ToList();
                if (endRooms.Count > 0)
                {
                    if (endRooms.Count > 1)
                        throw new Exception("More than 1 room with the same tile");

                    var endRoom = endRooms.Single();
                    newCorridor.EndRoom = endRoom;
                    newCorridor.EndTile = newCorridor.Tiles.Last();
                    break;
                }

                switch (direction)
                {
                    case Direction.UP:
                        currentTile = currentTile.Substract(y: 1);
                        break;
                    case Direction.RIGHT:
                        currentTile = currentTile.Add(x: 1);
                        break;
                    case Direction.DOWN:
                        currentTile = currentTile.Add(y: 1);
                        break;
                    case Direction.LEFT:
                        currentTile = currentTile.Substract(x: 1);
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
