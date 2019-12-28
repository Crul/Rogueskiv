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
        private const int MIN_SPACE_FOR_EACH_SIDE_TO_SPAWN_ENEMY = 1;
        private const int MIN_FOOD_SPAWN_DISTANCE = 20;
        private const int MIN_TORCH_SPAWN_DISTANCE = 10;
        private const int MIN_MAP_SPAWN_DISTANCE = 30;
        private const float MIN_AMULET_SPAWN_DISTANCE_FACTOR = 0.9f;
        private const float STAIRS_MIN_DISTANCE_FACTOR = 0.8f;
        private const int ENEMY_RADIUS = 6;

        private readonly IGameContext GameContext;
        private readonly IGameResult<IEntity> PreviousFloorResult;
        private readonly bool IsLastFloor;

        private readonly int EnemyNumber;
        private readonly int MinEnemySpeed;
        private readonly int MaxEnemySpeed;
        private readonly List<(int numAngles, float weight)> NumAnglesProbWeights;

        public static List<Point> NeighbourTilePositions { get; } = new List<Point>
        {
                               new Point(0, -1),
            new Point(-1,  0),                   new Point(1,  0),
                               new Point(0,  1),
        };

        public SpawnSys(
            IGameContext gameContext,
            float floorFactor,
            IGameResult<IEntity> previousFloorResult
        )
        {
            GameContext = gameContext;
            PreviousFloorResult = previousFloorResult;
            IsLastFloor = floorFactor == 1;

            // TODO magic numbers
            EnemyNumber = (int)(10 + (10f * floorFactor));          //  10 ... 20
            MinEnemySpeed = (int)(100f + (floorFactor * 25f));      // 100 ... 125
            MaxEnemySpeed = (int)(125f + (floorFactor * 50f));      // 125 ... 175
            NumAnglesProbWeights = new List<(int numAngles, float weight)> {
                (numAngles: 4, weight: 5),                           //   5 ...   5
                (numAngles: 8, weight: (floorFactor * 15f) - 10f),   //   0 ...   5
                (numAngles: 16, weight: (floorFactor * 7.5f) - 2.5f) //   0 ...   5
            };
        }

        public override void Init(Game game)
        {
            var boardComp = game.Entities.GetSingleComponent<BoardComp>();
            var tilePositions = boardComp
                .TilePositionsByTileId
                .Select(tileIdAndPosition => tileIdAndPosition.Value)
                .ToList();

            var tilesPosWithSpaceAround = tilePositions
                .Where(tilePos => HasSpaceAround(tilePositions, tilePos))
                .ToList();

            var playerTile = tilesPosWithSpaceAround[Luck.Next(tilesPosWithSpaceAround.Count)];
            game.AddEntity(CreatePlayer(playerTile));

            var measuredTiles = GetDistancesFrom(boardComp, playerTile)
                .Where(tile => tilePositions.Contains(tile.tilePos))
                .ToList();

            var enemiesCount = 0;
            while (enemiesCount < EnemyNumber)
            {
                var enemy = CreateEnemy(boardComp, measuredTiles);
                if (enemy == null)
                    continue;

                game.AddEntity(enemy);
                enemiesCount++;
            }

            // TODO avoid spawining 2 items in the same tile

            game.AddEntity(CreateFood(measuredTiles));
            game.AddEntity(CreateTorch(measuredTiles));
            game.AddEntity(CreateMapRevealer(measuredTiles));

            if (IsLastFloor)
                game.AddEntity(CreateAmulet(measuredTiles, tilesPosWithSpaceAround));
            else
                game.AddEntity(CreateDownStairs(measuredTiles, tilesPosWithSpaceAround));

            var isFirstFloor = PreviousFloorResult == null;
            if (!isFirstFloor)
                game.AddEntity(CreateUpStairs(playerTile));

            game.RemoveSystem(this);
        }

        public override void Update(EntityList entities, List<int> controls) =>
            throw new NotImplementedException();

        private List<IComponent> CreatePlayer(Point playerTilePos)
        {
            var previousPlayerEntity = GetPreviousPlayerEntity();

            return new List<IComponent> {
                new PlayerComp() {
                    VisualRange =
                        previousPlayerEntity?.GetComponent<PlayerComp>().VisualRange
                        ?? PlayerComp.INITIAL_VISUAL_RANGE
                },
                new HealthComp() {
                    MaxHealth = PlayerComp.INITIAL_PLAYER_HEALTH,
                    Health =
                        previousPlayerEntity?.GetComponent<HealthComp>().Health
                        ?? PlayerComp.INITIAL_PLAYER_HEALTH
                },
                new CurrentPositionComp(playerTilePos),
                new LastPositionComp(playerTilePos),
                new MovementComp(
                    frictionFactor: 1f / 10f,
                    bounceAmortiguationFactor: 2f / 3f,
                    radius: PlayerComp.PLAYER_RADIUS,
                    simpleBounce: false
                )
            };
        }

        private IEntity GetPreviousPlayerEntity()
        {
            if (PreviousFloorResult == null)
                return null;

            return PreviousFloorResult
                .Data
                .GetWithComponent<PlayerComp>()
                .Single();
        }

        private static List<(Point tilePos, int distance)> GetDistancesFrom(
            BoardComp boardComp, Point initialTile
        )
        {
            var currentDistance = 0;
            var visitingTiles = new List<Point> { initialTile };
            var measuredTiles = new List<(Point tilePos, int distance)>()
                { (initialTile, currentDistance) };

            var pendingTiles = boardComp
                .TileIdByTilePos
                .Keys
                .Where(tilePos => tilePos != initialTile)
                .ToList();

            while (pendingTiles.Count > 0)
            {
                currentDistance++;

                visitingTiles = visitingTiles
                    .SelectMany(tilePos => NeighbourTilePositions
                        .Select(neighbour => tilePos.Add(neighbour))
                    )
                    .Distinct()
                    .Where(neighbourId => pendingTiles.Contains(neighbourId))
                    .ToList();

                measuredTiles.AddRange(
                    visitingTiles.Select(neighbourTilePos => (
                        tilePos: neighbourTilePos,
                        distance: currentDistance
                    ))
                );

                visitingTiles.ForEach(tilePos => pendingTiles.Remove(tilePos));
            }

            return measuredTiles;
        }

        private List<IComponent> CreateEnemy(
            BoardComp boardComp,
            List<(Point tilePos, int distance)> tilePositionsAndDistances
        )
        {
            var enemyTilePos = GetRandomTilePos(tilePositionsAndDistances, MIN_ENEMY_SPAWN_DISTANCE);
            var enemySpeed = GetEnemySpeed(boardComp, enemyTilePos);
            if (!enemySpeed.HasValue)
                return null;

            return new List<IComponent>
            {
                new EnemyComp(),
                new CurrentPositionComp(enemyTilePos),
                new LastPositionComp(enemyTilePos),
                new MovementComp(
                    speed: enemySpeed.Value,
                    frictionFactor: 1,
                    bounceAmortiguationFactor: 1,
                    radius: ENEMY_RADIUS,
                    simpleBounce: true
                )
            };
        }

        private PointF? GetEnemySpeed(BoardComp boardComp, Point enemyTilePos)
        {
            var numAngles = NumAnglesProbWeights
                .OrderByDescending(napb => napb.weight * Luck.NextDouble())
                .First()
                .numAngles;

            var speed = (MinEnemySpeed + Luck.Next(MaxEnemySpeed - MinEnemySpeed)) / GameContext.GameFPS;
            var angleRatios = Enumerable.Range(1, numAngles).ToList();

            var randomizedAngleRatios = angleRatios.OrderBy(a => Luck.NextDouble()).ToList();
            while (randomizedAngleRatios.Any())
            {
                var angleRatio = randomizedAngleRatios.First();
                randomizedAngleRatios.Remove(angleRatio);

                if (!IsValidAngle(boardComp, enemyTilePos, numAngles, angleRatio))
                    continue;

                var angle = (float)angleRatio * 2 * Math.PI / numAngles;
                var speedX = (float)(speed * Math.Cos(angle));
                var speedY = (float)(speed * Math.Sin(angle));

                return new PointF(speedX, speedY);
            }

            return null;
        }

        private static bool IsValidAngle(BoardComp boardComp, Point enemyTilePos, int angleNum, int angleRatio)
        {
            var isRightOrLeft = angleRatio == angleNum
                || angleRatio == (angleNum / 2);

            if (isRightOrLeft)
            {
                for (var x = -MIN_SPACE_FOR_EACH_SIDE_TO_SPAWN_ENEMY; x <= MIN_SPACE_FOR_EACH_SIDE_TO_SPAWN_ENEMY; x++)
                    if (boardComp.WallsByTiles.ContainsKey(enemyTilePos.Add(x: x)))
                        return false;

                if (boardComp.WallsByTiles.ContainsKey(enemyTilePos.Add(y: 1))
                    && boardComp.WallsByTiles.ContainsKey(enemyTilePos.Substract(y: 1)))
                    return false;

                return true;
            }

            var nintyDegreesInRatio = angleNum / 4;
            var isUpOrDown = angleRatio + nintyDegreesInRatio == angleNum
                || angleRatio + nintyDegreesInRatio == (angleNum / 2);

            if (isUpOrDown)
            {
                for (var y = -MIN_SPACE_FOR_EACH_SIDE_TO_SPAWN_ENEMY; y <= MIN_SPACE_FOR_EACH_SIDE_TO_SPAWN_ENEMY; y++)
                    if (boardComp.WallsByTiles.ContainsKey(enemyTilePos.Add(y: y)))
                        return false;

                if (boardComp.WallsByTiles.ContainsKey(enemyTilePos.Add(x: 1))
                    && boardComp.WallsByTiles.ContainsKey(enemyTilePos.Substract(x: 1)))
                    return false;
            }

            return true;
        }

        private static IComponent CreateFood(
            List<(Point tilePos, int distance)> tilePositionsAndDistances
        )
        {
            var foodTilePos = GetRandomTilePos(tilePositionsAndDistances, MIN_FOOD_SPAWN_DISTANCE);

            return new FoodComp(foodTilePos);
        }

        private static IComponent CreateTorch(
            List<(Point tilePos, int distance)> tilePositionsAndDistances
        )
        {
            var torchTilePos = GetRandomTilePos(tilePositionsAndDistances, MIN_TORCH_SPAWN_DISTANCE);

            return new TorchComp(torchTilePos);
        }

        private static IComponent CreateMapRevealer(
            List<(Point tilePos, int distance)> tilePositionsAndDistances
        )
        {
            var mapRevealerTilePos = GetRandomTilePos(tilePositionsAndDistances, MIN_MAP_SPAWN_DISTANCE);

            return new MapRevealerComp(mapRevealerTilePos);
        }

        private static IComponent CreateAmulet(
            List<(Point tilePos, int distance)> tilePositionsAndDistances,
            List<Point> tilesWithSpaceAround
        )
        {
            var tilePosAndDistWithSpaceAround = tilePositionsAndDistances
                .Where(x => tilesWithSpaceAround.Contains(x.tilePos))
                .ToList();

            var maxDistance = tilePosAndDistWithSpaceAround.Max(tcd => tcd.distance);
            var minDistance = (int)(MIN_AMULET_SPAWN_DISTANCE_FACTOR * maxDistance);
            var tilePos = GetRandomTilePos(tilePosAndDistWithSpaceAround, minDistance);

            return new AmuletComp(tilePos);
        }

        private static IComponent CreateDownStairs(
            List<(Point tilePos, int distance)> tilePositionsAndDistances,
            List<Point> tilesWithSpaceAround
        )
        {
            var tilePosAndDistWithSpaceAround = tilePositionsAndDistances
                .Where(x => tilesWithSpaceAround.Contains(x.tilePos))
                .ToList();

            var maxDistance = tilePosAndDistWithSpaceAround.Max(tcd => tcd.distance);
            var minDistance = (int)(STAIRS_MIN_DISTANCE_FACTOR * maxDistance);

            var tilePos = GetRandomTilePos(tilePosAndDistWithSpaceAround, minDistance);

            return new DownStairsComp(tilePos);
        }

        private static IComponent CreateUpStairs(Point playerTilePos) =>
            new UpStairsComp(playerTilePos);

        private bool HasSpaceAround(List<Point> tilePositions, Point tilePos) =>
            BoardComp
                .NeighbourTilePositions
                .All(neighbour => tilePositions.Contains(tilePos.Add(neighbour)));

        private static Point GetRandomTilePos(
            List<(Point tilePos, int distance)> tilePositionsAndDistances,
            int minDistance
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

                    return tilePos;
                }
            }
        }
    }
}
