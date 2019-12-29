﻿using Rogueskiv.Core.Components;
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
        private readonly ISpawnConfig SpawnConfig;
        private readonly IGameResult<IEntity> PreviousFloorResult;

        public static List<Point> NeighbourTilePositions { get; } = new List<Point>
        {
                               new Point(0, -1),
            new Point(-1,  0),                   new Point(1,  0),
                               new Point(0,  1),
        };

        public SpawnSys(
            ISpawnConfig spawnConfig,
            IGameResult<IEntity> previousFloorResult
        )
        {
            SpawnConfig = spawnConfig;
            PreviousFloorResult = previousFloorResult;
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

            var tilePosAndDistances = GetDistancesFrom(boardComp, playerTile)
                .Where(tile => tilePositions.Contains(tile.tilePos))
                .ToList();

            var enemiesCount = 0;
            while (enemiesCount < SpawnConfig.EnemyNumber)
            {
                var enemy = CreateEnemy(boardComp, tilePosAndDistances);
                if (enemy == null)
                    continue;

                game.AddEntity(enemy);
                enemiesCount++;
            }

            var maxDistance = tilePosAndDistances.Max(tcd => tcd.distance);

            // TODO avoid spawining 2 items in the same tile

            game.AddEntity(CreateFood(tilePosAndDistances, maxDistance));
            game.AddEntity(CreateTorch(tilePosAndDistances, maxDistance));
            game.AddEntity(CreateMapRevealer(tilePosAndDistances, maxDistance));

            if (SpawnConfig.IsLastFloor)
                game.AddEntity(CreateAmulet(tilePosAndDistances, maxDistance));
            else
            {
                var tilePosAndDistWithSpaceAround = tilePosAndDistances
                    .Where(tpad => tilesPosWithSpaceAround.Contains(tpad.tilePos))
                    .ToList();

                game.AddEntity(CreateDownStairs(tilePosAndDistWithSpaceAround, maxDistance));
            }

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
                        ?? SpawnConfig.InitialPlayerVisualRange
                },
                new HealthComp() {
                    MaxHealth = SpawnConfig.InitialPlayerHealth,
                    Health =
                        previousPlayerEntity?.GetComponent<HealthComp>().Health
                        ?? SpawnConfig.InitialPlayerHealth
                },
                new CurrentPositionComp(playerTilePos),
                new LastPositionComp(playerTilePos),
                new BoundedMovementComp(
                    SpawnConfig.PlayerMaxSpeed,
                    SpawnConfig.PlayerStopSpeed,
                    frictionFactor: SpawnConfig.PlayerFrictionFactor,
                    bounceAmortiguationFactor: SpawnConfig.PlayerBounceAmortiguationFactor,
                    radius: SpawnConfig.PlayerRadius,
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
            var enemyTilePos = GetRandomTilePos(tilePositionsAndDistances, SpawnConfig.MinEnemySpawnDistance);
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
                    radius: SpawnConfig.EnemyRadius,
                    simpleBounce: true
                )
            };
        }

        private PointF? GetEnemySpeed(BoardComp boardComp, Point enemyTilePos)
        {
            var numAngles = SpawnConfig
                .EnemyNumAnglesProbWeights
                .OrderByDescending(napb => napb.weight * Luck.NextDouble())
                .First()
                .numAngles;

            var speed = (SpawnConfig.MinEnemySpeed
                + Luck.Next(SpawnConfig.MaxEnemySpeed - SpawnConfig.MinEnemySpeed));
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

        private bool IsValidAngle(BoardComp boardComp, Point enemyTilePos, int angleNum, int angleRatio)
        {
            var isRightOrLeft = angleRatio == angleNum
                || angleRatio == (angleNum / 2);

            if (isRightOrLeft)
            {
                for (var x = -SpawnConfig.MinSpaceToSpawnEnemy; x <= SpawnConfig.MinSpaceToSpawnEnemy; x++)
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
                for (var y = -SpawnConfig.MinSpaceToSpawnEnemy; y <= SpawnConfig.MinSpaceToSpawnEnemy; y++)
                    if (boardComp.WallsByTiles.ContainsKey(enemyTilePos.Add(y: y)))
                        return false;

                if (boardComp.WallsByTiles.ContainsKey(enemyTilePos.Add(x: 1))
                    && boardComp.WallsByTiles.ContainsKey(enemyTilePos.Substract(x: 1)))
                    return false;
            }

            return true;
        }

        private IComponent CreateFood(
            List<(Point tilePos, int distance)> tilePositionsAndDistances,
            int maxDistance
        )
        {
            var minDistance = (int)(SpawnConfig.MinFoodSpawnDistanceFactor * maxDistance);
            var foodTilePos = GetRandomTilePos(tilePositionsAndDistances, minDistance);

            return new FoodComp(SpawnConfig.MaxItemPickingTime, foodTilePos);
        }

        private IComponent CreateTorch(
            List<(Point tilePos, int distance)> tilePositionsAndDistances,
            int maxDistance
        )
        {
            var minDistance = (int)(SpawnConfig.MinTorchSpawnDistanceFactor * maxDistance);
            var torchTilePos = GetRandomTilePos(tilePositionsAndDistances, minDistance);

            return new TorchComp(SpawnConfig.MaxItemPickingTime, torchTilePos);
        }

        private IComponent CreateMapRevealer(
            List<(Point tilePos, int distance)> tilePositionsAndDistances,
            int maxDistance
        )
        {
            var minDistance = (int)(SpawnConfig.MinMapRevealerSpawnDistanceFactor * maxDistance);
            var mapRevealerTilePos = GetRandomTilePos(tilePositionsAndDistances, minDistance);

            return new MapRevealerComp(SpawnConfig.MaxItemPickingTime, mapRevealerTilePos);
        }

        private IComponent CreateAmulet(
            List<(Point tilePos, int distance)> tilePositionsAndDistances,
            int maxDistance
        )
        {
            var minDistance = (int)(SpawnConfig.MinAmuletSpawnFactor * maxDistance);
            var amuletTilePos = GetRandomTilePos(tilePositionsAndDistances, minDistance);

            return new AmuletComp(SpawnConfig.MaxItemPickingTime, amuletTilePos);
        }

        private IComponent CreateDownStairs(
            List<(Point tilePos, int distance)> tilePosAndDistWithSpaceAround,
            int maxDistance
        )
        {
            var minDistance = (int)(SpawnConfig.MinDownStairsSpawnFactor * maxDistance);
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
