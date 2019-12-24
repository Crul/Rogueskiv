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
            EnemyNumber = (int)Math.Pow(8 + (15f * floorFactor), 1.33f); //  15 ... 64
            MinEnemySpeed = (int)(75f + (floorFactor * 25f));            //  75 ... 100
            MaxEnemySpeed = (int)(150f + (floorFactor * 100f));          // 200 ... 300
            NumAnglesProbWeights = new List<(int numAngles, float weight)> {
                (numAngles: 4, weight: 5),
                (numAngles: 8, weight: (floorFactor * 20f) - 8f),
                (numAngles: 16, weight: (floorFactor * 40f) - 15f)
            };
        }

        public override void Init(Game game)
        {
            var boardComp = game.Entities.GetSingleComponent<BoardComp>();
            var tilePositions = boardComp
                .TilePositionsByTileId
                .Where(tileIdAndPosition =>
                    !game.Entities[tileIdAndPosition.Key].GetComponent<TileComp>().IsWall
                )
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

            Enumerable
                .Range(0, EnemyNumber)
                    .Select(i => CreateEnemy(measuredTiles))
                    .ToList()
                    .ForEach(enemy => game.AddEntity(enemy));

            // TODO avoid spawining 2 items in the same tile

            game.AddEntity(CreateFood(measuredTiles));
            game.AddEntity(CreateTorch(measuredTiles));
            game.AddEntity(CreateMap(measuredTiles));

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

            var playerPos = playerTilePos
                .Multiply(BoardComp.TILE_SIZE)
                .Add(BoardComp.TILE_SIZE / 2);

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
                new CurrentPositionComp(playerPos),
                new LastPositionComp(playerPos),
                new MovementComp(
                    frictionFactor: 1f / 5f,
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
            var initialTileEntityId = boardComp.TileIdByTilePos[initialTile];

            var measurePendingTileIds = boardComp
                .TileIdByTilePos
                .Values
                .Where(id => id != initialTileEntityId)
                .ToList();

            var currentDistance = 0;
            var measuredTiles = new List<(Point tilePos, int distance)>()
                { (initialTile, currentDistance) };

            while (measurePendingTileIds.Count > 0)
            {
                currentDistance++;
                var neighbourTileIds = measuredTiles
                    .SelectMany(tile => boardComp
                        .TilesNeighbours[tile.tilePos]
                        .ToList()
                        .Where(neighbourId => measurePendingTileIds.Contains(neighbourId))
                    )
                    .Distinct()
                    .ToList();

                measuredTiles.AddRange(
                    neighbourTileIds.Select(neighbourId => (
                        tilePos: boardComp.TilePositionsByTileId[neighbourId],
                        distance: currentDistance
                    ))
                );

                neighbourTileIds.ForEach(id => measurePendingTileIds.Remove(id));
            }

            return measuredTiles;
        }

        private List<IComponent> CreateEnemy(
            List<(Point tilePos, int distance)> tilePositionsAndDistances
        )
        {
            var enemyTilePos = GetRandomTilePos(
                tilePositionsAndDistances,
                MIN_ENEMY_SPAWN_DISTANCE
            );
            var enemyPos = enemyTilePos
                .Multiply(BoardComp.TILE_SIZE)
                .Add(Luck.Next(BoardComp.TILE_SIZE));

            return new List<IComponent>
            {
                new EnemyComp(),
                new CurrentPositionComp(enemyPos),
                new LastPositionComp(enemyPos),
                new MovementComp(
                    speed: GetEnemySpeed(),
                    frictionFactor: 1,
                    bounceAmortiguationFactor: 1,
                    radius: ENEMY_RADIUS,
                    simpleBounce: true
                )
            };
        }

        private PointF GetEnemySpeed()
        {
            var numAngles = NumAnglesProbWeights
                .OrderByDescending(napb => napb.weight * Luck.NextDouble())
                .First()
                .numAngles;

            var speed = (MinEnemySpeed + Luck.Next(MaxEnemySpeed - MinEnemySpeed)) / GameContext.GameFPS;

            var angles = Enumerable.Range(1, numAngles).Select(i => (float)i * 2 * Math.PI / numAngles).ToList();
            var angle = angles[Luck.Next(angles.Count)];

            var speedX = (float)(speed * Math.Cos(angle));
            var speedY = (float)(speed * Math.Sin(angle));

            return new PointF(speedX, speedY);
        }

        private static IComponent CreateFood(
            List<(Point tilePos, int distance)> tilePositionsAndDistances
        )
        {
            var foodTilePos = GetRandomTilePos(tilePositionsAndDistances, MIN_FOOD_SPAWN_DISTANCE)
                .Multiply(BoardComp.TILE_SIZE)
                .Add(BoardComp.TILE_SIZE / 2);

            return new FoodComp(foodTilePos);
        }

        private static IComponent CreateTorch(
            List<(Point tilePos, int distance)> tilePositionsAndDistances
        )
        {
            var torchTilePos = GetRandomTilePos(tilePositionsAndDistances, MIN_TORCH_SPAWN_DISTANCE)
                .Multiply(BoardComp.TILE_SIZE)
                .Add(BoardComp.TILE_SIZE / 2);

            return new TorchComp(torchTilePos);
        }

        private static IComponent CreateMap(
            List<(Point tilePos, int distance)> tilePositionsAndDistances
        )
        {
            var mapTilePos = GetRandomTilePos(tilePositionsAndDistances, MIN_MAP_SPAWN_DISTANCE)
                .Multiply(BoardComp.TILE_SIZE)
                .Add(BoardComp.TILE_SIZE / 2);

            return new MapComp(mapTilePos);
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
            var tilePos = GetRandomTilePos(tilePosAndDistWithSpaceAround, minDistance)
                .Multiply(BoardComp.TILE_SIZE)
                .Add(BoardComp.TILE_SIZE / 2);

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

            return CreateStairts(tilePos, tilePos => new DownStairsComp(tilePos));
        }

        private static IComponent CreateUpStairs(Point playerTilePos) =>
            CreateStairts(playerTilePos, tilePos => new UpStairsComp(tilePos));

        private bool HasSpaceAround(List<Point> tilePositions, Point tilePos) =>
            BoardComp
                .NeighbourTilePositions
                .All(neighbour => tilePositions.Contains(tilePos.Add(neighbour)));

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
