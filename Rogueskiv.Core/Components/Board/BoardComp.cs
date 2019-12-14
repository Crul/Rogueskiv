using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.Components.Walls;
using Seedwork.Core.Components;
using Seedwork.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Components.Board
{
    public class BoardComp : IComponent
    {
        private const int TILE_SIZE = 30; // TODO proper tile size

        private readonly List<(int x, int y)> NeighbourCoords = new List<(int x, int y)>
        {
            (-1, -1), (0, -1), (1, -1),
            (-1,  0),          (1,  0),
            (-1,  1), (0,  1), (1,  1),
        };

        public List<string> Board { get; set; }

        private readonly Dictionary<EntityId, (int x, int y)> CoordsByTileId;
        private readonly Dictionary<(int x, int y), EntityId> TileIdByCoords;
        private readonly Dictionary<(int x, int y), List<EntityId>> TilesNeighbours;
        private readonly Dictionary<(int x, int y), List<EntityId>> WallsByTiles;

        public BoardComp(string boardData)
        {
            Board = boardData.Split(Environment.NewLine).ToList();
            CoordsByTileId = new Dictionary<EntityId, (int x, int y)>();
            TileIdByCoords = new Dictionary<(int x, int y), EntityId>();
            TilesNeighbours = new Dictionary<(int x, int y), List<EntityId>>();
            WallsByTiles = new Dictionary<(int x, int y), List<EntityId>>();
        }

        public void AddTile(int x, int y, EntityId tileId)
        {
            var coords = (x, y);
            CoordsByTileId[tileId] = coords;
            TileIdByCoords[coords] = tileId;
            TilesNeighbours[coords] = new List<EntityId>();
            WallsByTiles[coords] = new List<EntityId>();
        }

        public void SetTilesNeighbours() =>
            CoordsByTileId.ToList().ForEach(cbti => SetTileNeighbours(cbti.Value));

        public void AddWall(IEntity wall, IWallComp wallComp)
        {
            var size = (wallComp.Size / TILE_SIZE);
            if (wallComp is HorizontalWallComp)
            {
                var init = (int)(wallComp.Position.X / TILE_SIZE);
                var y = (int)(wallComp.Position.Y / TILE_SIZE);
                if (wallComp is UpWallComp)
                    y -= 1;

                for (var x = init; x < init + size; x++)
                    WallsByTiles[(x, y)].Add(wall.Id);
            }
            else
            {
                var init = (int)(wallComp.Position.Y / TILE_SIZE);
                var x = (int)(wallComp.Position.X / TILE_SIZE);
                if (wallComp is LeftWallComp)
                    x -= 1;

                for (var y = init; y < init + size; y++)
                    WallsByTiles[(x, y)].Add(wall.Id);
            }
        }

        public List<EntityId> GetWallsIdsNear(PositionComp position)
        {
            var coords = (
                (int)Math.Floor(position.X / TILE_SIZE),
                (int)Math.Floor(position.Y / TILE_SIZE)
            );

            if (!WallsByTiles.ContainsKey(coords))
            {
                Console.WriteLine( // TODO 
                    $"WARNING: {position.X / TILE_SIZE}, {position.Y / TILE_SIZE} out of bounds {coords}"
                );
                return Enumerable.Empty<EntityId>().ToList();
            }

            var tilesCoords = TilesNeighbours[coords]
                .Select(tileId => CoordsByTileId[tileId])
                .ToList();

            tilesCoords.Add(coords);

            return tilesCoords.SelectMany(x => WallsByTiles[x]).Distinct().ToList();
        }

        private void SetTileNeighbours((int x, int y) tileCoords) =>
            NeighbourCoords
                .Select(nCoord => (
                    tileCoords.x + nCoord.x,
                    tileCoords.y + nCoord.y
                ))
                .Where(TileIdByCoords.ContainsKey)
                .Select(nCoord => TileIdByCoords[nCoord])
                .ToList()
                .ForEach(TilesNeighbours[tileCoords].Add);
    }
}
