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

        private readonly List<(int x, int y)> NeighbourCoords = new List<(int x, int y)>
        {
            (-1, -1), (0, -1), (1, -1),
            (-1,  0),          (1,  0),
            (-1,  1), (0,  1), (1,  1),
        };

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
            var tileCoords = game
                .Entities
                .GetWithComponent<TileComp>()
                .Select(e => e.GetComponent<TileComp>())
                .Select(tileComp => (
                    x: (int)(tileComp.X / BoardComp.TILE_SIZE),
                    y: (int)(tileComp.Y / BoardComp.TILE_SIZE)
                ))
                .ToList();

            var playerTile = (x: 0, y: 0);
            do
            {
                playerTile = tileCoords[Luck.Next(tileCoords.Count)];
            } while (!IsValidStairs(tileCoords, playerTile));

            game.AddEntity(CreatePlayer(playerTile));

            var tileCordsAndDistances = tileCoords
                .Select(tileCoord => (
                    tileCoord,
                    // TODO spawn distance taking into account the board
                    distance: Distance.Get(tileCoord.x - playerTile.x, tileCoord.y - playerTile.y)
                ))
                .ToList();

            Enumerable
                .Range(0, EnemyNumber)
                .Select(i => CreateEnemy(game, tileCordsAndDistances))
                .ToList()
                .ForEach(enemy => game.AddEntity(enemy));

            game.AddEntity(CreateDownStairs(tileCoords, tileCordsAndDistances));

            var isFirstFloor = PreviousFloorResult == null;
            if (!isFirstFloor)
                game.AddEntity(CreateUpStairs(playerTile));

            return false;
        }

        public override void Update(EntityList entities, List<int> controls) =>
            throw new NotImplementedException();

        private List<IComponent> CreatePlayer((int x, int y) playerTile)
        {
            var x = (BoardComp.TILE_SIZE / 2) + (playerTile.x * BoardComp.TILE_SIZE);
            var y = (BoardComp.TILE_SIZE / 2) + (playerTile.y * BoardComp.TILE_SIZE);

            return new List<IComponent> {
                new PlayerComp(),
                new HealthComp() { Health = GetPreviousHealth() ?? INITIAL_PLAYER_HEALTH },
                new CurrentPositionComp() { X = x, Y = y },
                new LastPositionComp() { X = x, Y = y },
                new MovementComp(){
                    FrictionFactor = 1f / 5f,
                    BounceAmortiguationFactor = 2f / 3f
                }
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

        private List<IComponent> CreateEnemy(
            Game game,
            List<((int x, int y) tileCoord, float distance)> tileCoordsAndDistances
        )
        {
            (int x, int y) = GetRandomPosition(tileCoordsAndDistances, MIN_ENEMY_SPAWN_DISTANCE);

            x *= BoardComp.TILE_SIZE;
            y *= BoardComp.TILE_SIZE;

            return new List<IComponent>
            {
                new EnemyComp(),
                new CurrentPositionComp() { X = x, Y = y },
                new LastPositionComp() { X = x, Y = y },
                new MovementComp() {
                    SpeedX = (50 + Luck.Next(100)) / GameContext.GameFPS,
                    SpeedY = (50 + Luck.Next(100)) / GameContext.GameFPS,
                    FrictionFactor = 1,
                    BounceAmortiguationFactor = 1
                }
            };
        }

        private IComponent CreateDownStairs(
            List<(int x, int y)> tileCoords,
            List<((int x, int y) tileCoord, float distance)> tileCoordsAndDistances
        )
        {
            var maxDistance = tileCoordsAndDistances.Max(tcd => tcd.distance);
            var minDistance = (int)(STAIRS_MIN_DISTANCE_FACTOR * maxDistance);
            (int x, int y) = (0, 0);
            do
            {
                // TODO what if no available ???
                (x, y) = GetRandomPosition(tileCoordsAndDistances, minDistance);
            } while (!IsValidStairs(tileCoords, (x, y)));

            x *= BoardComp.TILE_SIZE;
            y *= BoardComp.TILE_SIZE;

            return CreateStairts<DownStairsComp>((x, y));
        }

        private IComponent CreateUpStairs((int x, int y) playerTile)
        {
            var x = (playerTile.x * BoardComp.TILE_SIZE);
            var y = (playerTile.y * BoardComp.TILE_SIZE);

            return CreateStairts<UpStairsComp>((x, y));
        }

        private bool IsValidStairs(
            List<(int x, int y)> tileCoords,
            (int x, int y) position
        ) =>
            NeighbourCoords
                .All(neighbour =>
                    tileCoords.Contains(
                        (
                            position.x + neighbour.x,
                            position.y + neighbour.y
                        )
                    )
                );

        private IComponent CreateStairts<T>((int x, int y) position)
            where T : StairsComp, new() =>
            new T()
            {
                X = (BoardComp.TILE_SIZE / 2) + position.x,
                Y = (BoardComp.TILE_SIZE / 2) + position.y
            };

        private static (int x, int y) GetRandomPosition(
            List<((int x, int y) tileCoord, float distance)> tileCoordsAndDistances,
            int minDistance
        )
        {
            var candidates = tileCoordsAndDistances
                .Where(tcd => tcd.distance > minDistance)
                .Select(tcd => tcd.tileCoord)
                .ToList();

            return candidates[Luck.Next(candidates.Count)];
        }
    }
}
