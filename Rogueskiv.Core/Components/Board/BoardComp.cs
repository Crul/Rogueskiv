using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.Components.Walls;
using Seedwork.Core.Components;
using Seedwork.Core.Entities;
using Seedwork.Crosscutting;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Rogueskiv.Core.Components.Board
{
    public class BoardComp : IComponent
    {
        public const int TILE_SIZE = 32;
        public static List<Point> NeighbourTilePositions { get; } = new List<Point>
        {
            new Point(-1, -1), new Point(0, -1), new Point(1, -1),
            new Point(-1,  0),                   new Point(1,  0),
            new Point(-1,  1), new Point(0,  1), new Point(1,  1),
        };

        private List<string> board;
        public List<string> Board
        {
            get => board;
            set
            {
                board = value;
                BoardSize = GetSize(board);
            }
        }
        public Size BoardSize { get; private set; }

        public IDictionary<Point, EntityId> TileIdByTilePos { get; }
        public IDictionary<EntityId, Point> TilePositionsByTileId { get; }
        public IDictionary<Point, List<EntityId>> TilesNeighbours { get; }
        public IDictionary<Point, List<EntityId>> WallsByTiles { get; }

        private readonly IDictionary<Point, List<EntityId>> EntitiesByTiles;

        public BoardComp()
        {
            TilePositionsByTileId = new Dictionary<EntityId, Point>();
            TileIdByTilePos = new Dictionary<Point, EntityId>();
            TilesNeighbours = new Dictionary<Point, List<EntityId>>();
            WallsByTiles = new Dictionary<Point, List<EntityId>>();
            EntitiesByTiles = new Dictionary<Point, List<EntityId>>();
        }

        public void UpdateEntity(
            EntityId entityId,
            CurrentPositionComp currentPosComp,
            LastPositionComp lastPosComp
        )
        {
            var lastTilePos = lastPosComp.TilePos;
            var currentTilePos = currentPosComp.TilePos;

            if (currentTilePos == lastTilePos)
                return;

            var lastTileEntityList = EntitiesByTiles[lastTilePos];
            if (lastTileEntityList.Contains(entityId))
                lastTileEntityList.Remove(entityId);

            EntitiesByTiles[currentTilePos].Add(entityId);
        }

        public void RemoveEntity(EntityId entityId, PositionComp positionComp) =>
            EntitiesByTiles[positionComp.TilePos].Remove(entityId);

        public List<EntityId> GetEntityIdsNear(EntityId entityId, PositionComp positionComp) =>
            GetIdsNear(EntitiesByTiles, positionComp, entityId);

        public bool IsTileOrWall(Point tilePos) =>
            TileIdByTilePos.ContainsKey(tilePos) || WallsByTiles.ContainsKey(tilePos);

        public void AddTile(Point tilePos, EntityId tileId)
        {
            TilePositionsByTileId[tileId] = tilePos;
            TileIdByTilePos[tilePos] = tileId;
            TilesNeighbours[tilePos] = new List<EntityId>();
            EntitiesByTiles[tilePos] = new List<EntityId>();
        }

        public void SetTilesNeighbours() =>
            TilePositionsByTileId.ToList().ForEach(cbti => SetTileNeighbours(cbti.Value));

        public void AddWall(IEntity wall, IWallComp wallComp)
        {
            IEnumerable<Point> wallTilePositions;
            var size = (wallComp.Size / TILE_SIZE);
            if (wallComp is HorizontalWallComp)
            {
                var init = wallComp.TilePos.X;
                var y = wallComp.TilePos.Y;
                wallTilePositions = Enumerable
                    .Range(init, size)
                    .Select(x => new Point(x, y));
            }
            else
            {
                var init = wallComp.TilePos.Y;
                var x = wallComp.TilePos.X;
                wallTilePositions = Enumerable
                    .Range(init, size)
                    .Select(y => new Point(x, y));
            }

            wallTilePositions.ToList().ForEach(wallTilePos =>
            {
                if (!WallsByTiles.ContainsKey(wallTilePos))
                    WallsByTiles[wallTilePos] = new List<EntityId>();

                WallsByTiles[wallTilePos].Add(wall.Id);
            });
        }

        public List<EntityId> GetWallsIdsNear(PositionComp positionComp) =>
            GetIdsNear(WallsByTiles, positionComp);

        private static List<EntityId> GetIdsNear(
            IDictionary<Point, List<EntityId>> EntityIdsByTilePos,
            PositionComp positionComp,
            EntityId entityId = default
        )
        {
            var tilePos = positionComp.TilePos;
            var tilePositions = NeighbourTilePositions
                .Select(neighbourTilePos => tilePos.Add(neighbourTilePos))
                .ToList();

            tilePositions.Add(tilePos);

            return tilePositions
                .Where(tilePos => EntityIdsByTilePos.ContainsKey(tilePos)) // TODO diagonal walls ??
                .SelectMany(tilePos => EntityIdsByTilePos[tilePos])
                .Distinct()
                .Where(id => id != entityId)
                .ToList();
        }

        private void SetTileNeighbours(Point tilePos) =>
            NeighbourTilePositions
                .Select(neighbourTilePos => tilePos.Add(neighbourTilePos))
                .Where(TileIdByTilePos.ContainsKey)
                .Select(neighbourTilePos => TileIdByTilePos[neighbourTilePos])
                .ToList()
                .ForEach(TilesNeighbours[tilePos].Add);

        private static Size GetSize(List<string> board) =>
            new Size(board[0].Length, board.Count);
    }
}
