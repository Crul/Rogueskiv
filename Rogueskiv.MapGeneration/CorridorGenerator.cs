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

        // TODO refactor XxxxxProbWeights
        private static readonly List<(int width, float weight)> CorridorWidthProbWeights =
            new List<(int width, float weight)>
            {
                ( width: 1, weight: 0.5f ),
                ( width: 2, weight: 3 ),
                ( width: 3, weight: 8 ),
                ( width: 4, weight: 1 )
            };

        private readonly static IDictionary<Direction, Func<Point, Point>> NextTileFn =
            new Dictionary<Direction, Func<Point, Point>>
            {
                { Direction.UP, tile => tile.Substract(y: 1)},
                { Direction.RIGHT, tile => tile.Add(x: 1) },
                { Direction.DOWN, tile => tile.Add(y: 1) },
                { Direction.LEFT, tile => tile.Substract(x: 1) },
            };

        private readonly static IDictionary<Direction, Func<Point, Func<int, Point>>> GetTurnedTileFn =
            new Dictionary<Direction, Func<Point, Func<int, Point>>>
            {
                { Direction.UP, tile => (i => tile.Add(y: i)) },
                { Direction.DOWN, tile => (i => tile.Substract(y: i)) },
                { Direction.LEFT, tile => (i => tile.Add(x: i)) },
                { Direction.RIGHT, tile => (i => tile.Substract(x: i)) },
            };

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

                var added = CreateCorridor(mapParams, rooms, corridors);
                if (added && AreAllConnected(rooms))
                    break;
            }

            return corridors;
        }

        private static bool CreateCorridor(
            MapGenerationParams mapParams, List<Room> rooms, List<Corridor> corridors
        )
        {
            var corridorWidth = CorridorWidthProbWeights
                .OrderByDescending(cwpw => cwpw.weight * Luck.NextDouble())
                .First()
                .width;

            var wideIndexList = Enumerable.Range(0, corridorWidth).ToList();

            var (startRoom, startTiles, direction) =
                GetStartTiles(rooms, corridors, corridorWidth, wideIndexList);

            if (startRoom == null)
                return false;

            var newCorridor = new Corridor();
            newCorridor.Rooms.Add(startRoom);

            var endRooms = ExpandCorridor(
                mapParams, corridors, newCorridor, corridorWidth, wideIndexList, startTiles, direction, rooms
            );
            if (endRooms == null || !endRooms.Any())
                return false;

            startRoom.Corridors.Add(newCorridor);

            newCorridor.Rooms.AddRange(endRooms);
            endRooms.ForEach(endRoom => endRoom.Corridors.Add(newCorridor));

            corridors.Add(newCorridor);

            return true;
        }

        #region StartTiles
        private static (Room room, List<Point>, Direction direction) GetStartTiles(
            List<Room> rooms,
            List<Corridor> corridors,
            int corridorWidth,
            List<int> wideIndexList
        )
        {
            var minNumberOfCorridors = rooms.Min(room => room.Corridors.Count);
            var startRoomCanddiates = (minNumberOfCorridors == 0)
                ? rooms.Where(room => room.Corridors.Count == 0).ToList()
                : rooms;

            var room = startRoomCanddiates[Luck.Next(startRoomCanddiates.Count)];
            var availableTiles = new List<(List<Point> tiles, Direction direction)>();

            var upTilesList = GetUpOrDownStartTiles(room, rooms, corridors, corridorWidth, wideIndexList, upOrDown: true);
            upTilesList.ForEach(upTiles => availableTiles.Add((upTiles, Direction.UP)));

            var downTilesList = GetUpOrDownStartTiles(room, rooms, corridors, corridorWidth, wideIndexList, upOrDown: false);
            downTilesList.ForEach(downTiles => availableTiles.Add((downTiles, Direction.DOWN)));

            var leftTiles = GetLeftOrRightStartTiles(room, rooms, corridors, corridorWidth, wideIndexList, leftOrRight: true);
            leftTiles.ForEach(leftTiles => availableTiles.Add((leftTiles, Direction.LEFT)));

            var rightTilesList = GetLeftOrRightStartTiles(room, rooms, corridors, corridorWidth, wideIndexList, leftOrRight: false);
            rightTilesList.ForEach(rightTiles => availableTiles.Add((rightTiles, Direction.RIGHT)));

            if (!availableTiles.Any())
                return (default, default, default);

            var (startTiles, direction) = availableTiles[Luck.Next(availableTiles.Count)];

            return (room, startTiles, direction);
        }

        private static List<List<Point>> GetUpOrDownStartTiles(
            Room room,
            List<Room> rooms,
            List<Corridor> corridors,
            int corridorWidth,
            List<int> indexList,
            bool upOrDown
        )
        {
            var deltaY = upOrDown ? -1 : 1;
            var initialStartTile = upOrDown ? room.TilePos : room.TilePos.Add(y: room.Size.Height - 1);
            var startTilesList = new List<List<Point>>();
            for (var x = 0; x < room.Size.Width - corridorWidth; x++)
            {
                var currentInitialTile = initialStartTile.Add(x: x, y: deltaY);
                var candidateStartTiles = indexList.Select(idx => currentInitialTile.Add(x: idx)).ToList();
                var areValidCandidates = candidateStartTiles.All(tilePos => !IsOccupied(tilePos, rooms, corridors));

                if (areValidCandidates)
                    startTilesList.Add(candidateStartTiles);
            }

            return startTilesList;
        }

        private static List<List<Point>> GetLeftOrRightStartTiles(
            Room room,
            List<Room> rooms,
            List<Corridor> corridors,
            int corridorWidth,
            List<int> indexList,
            bool leftOrRight
        )
        {
            var deltaX = leftOrRight ? -1 : 1;
            var initialStartTile = leftOrRight ? room.TilePos : room.TilePos.Add(x: room.Size.Width - 1);
            var startTilesList = new List<List<Point>>();
            for (var y = 0; y < room.Size.Height - corridorWidth; y++)
            {
                var currentInitialTile = initialStartTile.Add(x: deltaX, y: y);
                var candidateStartTiles = indexList.Select(idx => currentInitialTile.Add(y: idx)).ToList();
                var areValidCandidates = candidateStartTiles.All(tilePos => !IsOccupied(tilePos, rooms, corridors));

                if (areValidCandidates)
                    startTilesList.Add(candidateStartTiles);
            }

            return startTilesList;
        }

        #endregion

        #region Expansion
        private static List<Room> ExpandCorridor(
            MapGenerationParams mapParams,
            List<Corridor> corridors,
            Corridor corridor,
            int corridorWidth,
            List<int> wideIndexList,
            List<Point> startTiles,
            Direction direction,
            List<Room> rooms
        )
        {
            var currentTiles = startTiles;
            var straightPathLength = 0;
            while (true)
            {
                var areCurrentTilesValid = currentTiles
                    .All(tilePos =>
                        mapParams.IsTileInBounds(tilePos)
                        && !corridor.Tiles.Contains(tilePos)
                    );

                if (!areCurrentTilesValid)
                    return null;

                corridor.Tiles.AddRange(currentTiles);

                var endRooms = CheckEndRooms(rooms, corridors, corridor, currentTiles, direction, wideIndexList);
                if (endRooms.Any())
                    return endRooms;

                var canTurn = straightPathLength > corridorWidth;
                var hasTurned = false;
                if (canTurn && mapParams.CorridorTurnCheck())
                {
                    var (newCurrentTiles, turnToDirection) = TurnCorridor(direction, wideIndexList, currentTiles);
                    hasTurned = newCurrentTiles != null;
                    if (hasTurned)
                    {
                        direction = turnToDirection;
                        currentTiles = newCurrentTiles;
                        straightPathLength = 0;
                    }
                }

                if (!hasTurned)
                    straightPathLength++;

                currentTiles = currentTiles.Select(NextTileFn[direction]).ToList();
            }
        }

        private static (List<Point> newCurrentTiles, Direction turnToDirection) TurnCorridor(
            Direction currentDirection,
            List<int> wideIndexList,
            List<Point> currentTiles
        )
        {
            var turnToDirection = (currentDirection == Direction.UP || currentDirection == Direction.DOWN)
                ? (Luck.NextDouble() > 0.5 ? Direction.LEFT : Direction.RIGHT)
                : (Luck.NextDouble() > 0.5 ? Direction.UP : Direction.DOWN);

            var turnedTiles = GetTurnedTiles(currentDirection, wideIndexList, currentTiles, turnToDirection);

            return (turnedTiles, turnToDirection);
        }

        private static List<Point> GetTurnedTiles(
            Direction currentDirection,
            List<int> wideIndexList,
            List<Point> currentTiles,
            Direction turnToDirection
        )
        {
            var sortedCurrentTiles = currentTiles.OrderBy(tile => tile.X).ThenBy(tile => tile.Y);
            var initialTile = (turnToDirection == Direction.LEFT || turnToDirection == Direction.UP)
                ? sortedCurrentTiles.First()
                : sortedCurrentTiles.Last();

            var getTurnedTile = GetTurnedTileFn[currentDirection](initialTile);

            return wideIndexList.Select(getTurnedTile).ToList();
        }

        private static List<Room> CheckEndRooms(
            List<Room> rooms,
            List<Corridor> corridors,
            Corridor corridor,
            List<Point> currentTiles,
            Direction direction,
            List<int> wideIndexList
        )
        {
            var (turnedTilesDir1, turnedTilesDir2) =
                (direction == Direction.UP || direction == Direction.DOWN)
                    ? (Direction.LEFT, Direction.RIGHT)
                    : (Direction.UP, Direction.DOWN);

            var turnedTiles1 =
                    GetTurnedTiles(direction, wideIndexList, currentTiles, turnedTilesDir1)
                        .Select(NextTileFn[turnedTilesDir1])
                        .ToList();

            var turnedTiles2 =
                    GetTurnedTiles(direction, wideIndexList, currentTiles, turnedTilesDir2)
                        .Select(NextTileFn[turnedTilesDir2])
                        .ToList();

            var pipas1 = rooms
                .Where(room =>
                    !corridor.Rooms.Contains(room)
                    && (
                        currentTiles.Select(NextTileFn[direction]).Any(room.HasTile)
                        || turnedTiles1.Any(room.HasTile)
                        || turnedTiles2.Any(room.HasTile)
                    )
                ).ToList();

            var endRooms = rooms
                .Where(room =>
                    !corridor.Rooms.Contains(room)
                    && (
                        currentTiles.Select(NextTileFn[direction]).Any(room.HasTile)
                        || turnedTiles1.Any(room.HasTile)
                        || turnedTiles2.Any(room.HasTile)
                    )
                )
                .Concat(
                    corridors.Where(
                        otherCorr => currentTiles.Select(NextTileFn[direction]).Any(otherCorr.Tiles.Contains)
                    ).Concat(
                        corridors.Where(otherCorr => currentTiles.Any(otherCorr.Tiles.Contains))
                    ).SelectMany(
                        corridor => corridor.Rooms
                    )
                )
                .Distinct()
                .ToList();

            return endRooms;
        }

        #endregion

        #region Checks
        private static bool IsOccupied(
            Point tile,
            List<Room> rooms,
            List<Corridor> corridors,
            Corridor corridor = null
        ) =>
            rooms.Any(room => room.HasTile(tile))
                || corridors.Any(corridor => corridor.Tiles.Contains(tile))
                || (corridor != null && corridor.Tiles.Contains(tile));

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
                .SelectMany(corridor => corridor.Rooms)
                .Distinct()
                .Where(room => !room.Visited)
                .ToList()
                .ForEach(VisitRoom);
        }

        #endregion
    }
}
