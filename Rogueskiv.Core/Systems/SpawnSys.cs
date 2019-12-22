using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Seedwork.Core;
using Seedwork.Core.Components;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using Seedwork.Crosscutting;
using Seedwork.Engine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    class SpawnSys : BaseSystem
    {
        private const int MIN_ENEMY_SPAWN_DISTANCE = 5;
        private const float STAIRS_MIN_DISTANCE_FACTOR = 0.8f;
        private const int INITIAL_PLAYER_HEALTH = 100;

        private readonly IGameContext GameContext;
        private readonly IGameResult<IEntity> PreviousFloorResult;
        private readonly int EnemyNumber;

        public SpawnSys(
            IGameContext gameContext,
            IGameResult<IEntity> previousFloorResult,
            int enemyNumber
        )
        {
            GameContext = gameContext;
            PreviousFloorResult = previousFloorResult;
            EnemyNumber = enemyNumber;
        }

        public override bool Init(Game game)
        {
            var boardComp = game
                .Entities
                .GetWithComponent<BoardComp>()
                .Single()
                .GetComponent<BoardComp>();

            var tilePositions = boardComp.TileIdByTilePos.Keys.ToList();

            var playerTile = new Point();
            do
            {
                playerTile = tilePositions[Luck.Next(tilePositions.Count)];
            } while (!IsValidStairs(boardComp, tilePositions, playerTile));

            game.AddEntity(CreatePlayer(playerTile));

            var measuredTiles = GetDistancesFrom(boardComp, playerTile);

            Enumerable
                .Range(0, EnemyNumber)
                    .Select(i => CreateEnemy(measuredTiles))
                    .ToList()
                    .ForEach(enemy => game.AddEntity(enemy));

            game.AddEntity(CreateDownStairs(boardComp, tilePositions, measuredTiles));

            var isFirstFloor = PreviousFloorResult == null;
            if (!isFirstFloor)
                game.AddEntity(CreateUpStairs(playerTile));

            return false;
        }

        public override void Update(EntityList entities, List<int> controls) =>
            throw new NotImplementedException();

        private List<IComponent> CreatePlayer(Point playerTilePos)
        {

            var playerPos = playerTilePos
                .Multiply(BoardComp.TILE_SIZE)
                .Add(BoardComp.TILE_SIZE / 2);

            return new List<IComponent> {
                new PlayerComp(),
                new HealthComp() {
                    MaxHealth = INITIAL_PLAYER_HEALTH,
                    Health = GetPreviousHealth() ?? INITIAL_PLAYER_HEALTH
                },
                new CurrentPositionComp(playerPos),
                new LastPositionComp(playerPos),
                new MovementComp(
                    frictionFactor: 1f / 5f,
                    bounceAmortiguationFactor: 2f / 3f
                )
            };
        }

        private int? GetPreviousHealth()
        {
            if (PreviousFloorResult == null)
                return null;

            var previousPlayerComp = PreviousFloorResult
                .Data
                .GetWithComponent<PlayerComp>()
                .Single();

            var previousPlayerHealtComp = previousPlayerComp
                .GetComponent<HealthComp>();

            return previousPlayerHealtComp.Health;
        }

        private static List<(Point tilePos, int distance)> GetDistancesFrom(
            BoardComp boardComp, Point initialTile
        )
        {
            var initialTileEntityId = boardComp.TileIdByTilePos[initialTile];

            var measurePendignTileIds = boardComp
                .TileIdByTilePos
                .Values
                .Where(id => id != initialTileEntityId)
                .ToList();

            var currentDistance = 0;
            var measuredTiles = new List<(Point tilePos, int distance)>()
                { (initialTile, currentDistance) };

            while (measurePendignTileIds.Count > 0)
            {
                currentDistance++;
                var neighbourTileIds = measuredTiles
                    .SelectMany(tile => boardComp
                        .TilesNeighbours[tile.tilePos]
                        .ToList()
                        .Where(neighbourId => measurePendignTileIds.Contains(neighbourId))
                    )
                    .Distinct()
                    .ToList();

                measuredTiles.AddRange(
                    neighbourTileIds.Select(neighbourId => (
                        tilePos: boardComp.TilePositionsByTileId[neighbourId],
                        distance: currentDistance
                    ))
                );

                neighbourTileIds.ForEach(id => measurePendignTileIds.Remove(id));
            }

            return measuredTiles;
        }

        private List<IComponent> CreateEnemy(
            List<(Point tilePos, int distance)> tilePositionsAndDistances
        )
        {
            var enemyTilePos = GetRandomTilePos(tilePositionsAndDistances, MIN_ENEMY_SPAWN_DISTANCE);
            var enemyPos = enemyTilePos.Multiply(BoardComp.TILE_SIZE);

            return new List<IComponent>
            {
                new EnemyComp(),
                new CurrentPositionComp(enemyPos),
                new LastPositionComp(enemyPos),
                new MovementComp(
                    speed: new PointF(
                        (50 + Luck.Next(100)) / GameContext.GameFPS,
                        (50 + Luck.Next(100)) / GameContext.GameFPS
                    ),
                    frictionFactor: 1,
                    bounceAmortiguationFactor: 1
                )
            };
        }

        private static IComponent CreateDownStairs(
            BoardComp boardComp,
            List<Point> tilePositions,
            List<(Point tilePos, int distance)> tilePositionsAndDistances
        )
        {
            var maxDistance = tilePositionsAndDistances.Max(tcd => tcd.distance);
            var minDistance = (int)(STAIRS_MIN_DISTANCE_FACTOR * maxDistance);

            var tilePos = GetRandomTilePos(
                tilePositionsAndDistances,
                minDistance,
                isValidTilePos: tilePos => IsValidStairs(boardComp, tilePositions, tilePos)
            );

            return CreateStairts(tilePos, tilePos => new DownStairsComp(tilePos));
        }

        private static IComponent CreateUpStairs(Point playerTilePos) =>
            CreateStairts(playerTilePos, tilePos => new UpStairsComp(tilePos));

        private static bool IsValidStairs(
            BoardComp boardComp, List<Point> tilePositions, Point tilePos
        ) => boardComp
                .NeighbourTilePositions
                .All(neighbour =>
                    tilePositions.Contains(
                        new Point(
                            tilePos.X + neighbour.X,
                            tilePos.Y + neighbour.Y
                        )
                    )
                );

        private static IComponent CreateStairts<T>(Point tilePos, Func<PointF, T> createStairs)
            where T : StairsComp
        {
            var position = tilePos
                .Multiply(BoardComp.TILE_SIZE)
                .Add(BoardComp.TILE_SIZE / 2);

            return createStairs(position);
        }

        private static Point GetRandomTilePos(
            List<(Point tilePos, int distance)> tilePositionsAndDistances,
            int minDistance,
            Func<Point, bool> isValidTilePos = null
        )
        {
            if (tilePositionsAndDistances.Count == 0)
                throw new ArgumentException("SpawnSys.GetRandomPosition: empty tilePositionsAndDistances");

            var tilePos = new Point();
            var candidates = new List<Point>();

            while (true)
            {
                while (candidates.Count == 0)
                {
                    candidates = tilePositionsAndDistances
                        .Where(tcd => tcd.distance > minDistance)
                        .Select(tcd => tcd.tilePos)
                        .ToList();

                    minDistance--;
                    if (minDistance < 0)
                        throw new Exception("SpawnSys.CreateDownStaris: not tile available");
                }

                while (candidates.Count > 0)
                {
                    tilePos = candidates[Luck.Next(candidates.Count)];
                    candidates.Remove(tilePos);

                    if (isValidTilePos?.Invoke(tilePos) ?? true)
                        return tilePos;
                }
            }
        }
    }
}
