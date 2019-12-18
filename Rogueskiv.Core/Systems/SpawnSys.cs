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
        private const int ENEMY_NUMBER = 30;
        private const int MIN_ENEMY_SPAWN_DISTANCE = 5;
        private const float STAIRS_MIN_DISTANCE_FACTOR = 0.8f;

        private readonly IGameContext GameContext;

        public SpawnSys(IGameContext gameContext) => GameContext = gameContext;

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

            var playerTile = tileCoords[Luck.Next(tileCoords.Count)];

            game.AddEntity(CreatePlayer(playerTile));

            Enumerable
                .Range(0, ENEMY_NUMBER)
                .Select(i => CreateEnemy(game, tileCoords, playerTile))
                .ToList()
                .ForEach(enemy => game.AddEntity(enemy));

            game.AddEntity(CreateStairs(tileCoords, playerTile));

            return false;
        }

        public override void Update(EntityList entities, IEnumerable<int> controls) =>
            throw new NotImplementedException();

        private List<IComponent> CreatePlayer((int x, int y) playerTile)
        {
            var x = (BoardComp.TILE_SIZE / 2) + (playerTile.x * BoardComp.TILE_SIZE);
            var y = (BoardComp.TILE_SIZE / 2) + (playerTile.y * BoardComp.TILE_SIZE);

            return new List<IComponent> {
                new PlayerComp(),
                new HealthComp() { Health = 100 },
                new CurrentPositionComp() { X = x, Y = y },
                new LastPositionComp() { X = x, Y = y },
                new MovementComp(){
                    FrictionFactor = 1f / 5f,
                    BounceAmortiguationFactor = 2f / 3f
                }
            };
        }

        private List<IComponent> CreateEnemy(
            Game game, List<(int x, int y)> tileCoords, (int x, int y) playerTile
        )
        {
            (int x, int y) = GetRandomPosition(tileCoords, playerTile, MIN_ENEMY_SPAWN_DISTANCE);

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

        private IComponent CreateStairs(
            List<(int x, int y)> tileCoords, (int x, int y) playerTile
        )
        {
            var maxDistance = tileCoords
                .Max(tile => Distance.Get(tile.x - playerTile.x, tile.y - playerTile.y));

            var minDistance = (int)(STAIRS_MIN_DISTANCE_FACTOR * maxDistance);
            (int x, int y) = GetRandomPosition(tileCoords, playerTile, minDistance);

            return new StairsComp() { X = x, Y = y };
        }

        private static (int x, int y) GetRandomPosition(
            List<(int x, int y)> tileCoords, (int x, int y) playerTile, int minDistance
        )
        {
            (int x, int y) tile;
            float distance;
            do
            {
                tile = tileCoords[Luck.Next(tileCoords.Count)];
                distance = Distance.Get(tile.x - playerTile.x, tile.y - playerTile.y);

            } while (distance < minDistance);

            var x = (BoardComp.TILE_SIZE / 2) + (tile.x * BoardComp.TILE_SIZE);
            var y = (BoardComp.TILE_SIZE / 2) + (tile.y * BoardComp.TILE_SIZE);

            return (x, y);
        }
    }
}
