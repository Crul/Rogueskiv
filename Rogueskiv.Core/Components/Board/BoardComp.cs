﻿using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.Components.Walls;
using Seedwork.Core.Components;
using Seedwork.Core.Entities;
using System;
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
            GetIdsNear(EntitiesByTiles, entityId, positionComp);

        public void AddTile(Point tilePos, EntityId tileId)
        {
            TilePositionsByTileId[tileId] = tilePos;
            TileIdByTilePos[tilePos] = tileId;
            TilesNeighbours[tilePos] = new List<EntityId>();
            WallsByTiles[tilePos] = new List<EntityId>();
            EntitiesByTiles[tilePos] = new List<EntityId>();
        }

        public void SetTilesNeighbours() =>
            TilePositionsByTileId.ToList().ForEach(cbti => SetTileNeighbours(cbti.Value));

        public void AddWall(IEntity wall, IWallComp wallComp)
        {
            var size = (wallComp.Size / TILE_SIZE);
            if (wallComp is HorizontalWallComp)
            {
                var init = wallComp.PositionComp.TilePos.X;
                var y = wallComp.PositionComp.TilePos.Y;

                for (var x = init; x < init + size; x++)
                    WallsByTiles[new Point(x, y)].Add(wall.Id);

                // because convexe corners and diagonal movement
                AddWallSafe(init - 1, y, wall.Id);
                AddWallSafe(init + size, y, wall.Id);
            }
            else
            {
                var init = wallComp.PositionComp.TilePos.Y;
                var x = wallComp.PositionComp.TilePos.X;

                for (var y = init; y < init + size; y++)
                    WallsByTiles[new Point(x, y)].Add(wall.Id);

                // because convexe corners and diagonal movement
                AddWallSafe(x, init - 1, wall.Id);
                AddWallSafe(x, init + size, wall.Id);
            }
        }

        private void AddWallSafe(int tileX, int tileY, EntityId wallId)
        {
            var tilePos = new Point(tileX, tileY);
            if (WallsByTiles.ContainsKey(tilePos))
                WallsByTiles[tilePos].Add(wallId);
        }

        public List<EntityId> GetWallsIdsNear(EntityId entityId, PositionComp positionComp) =>
            GetIdsNear(WallsByTiles, entityId, positionComp);

        private List<EntityId> GetIdsNear(
            IDictionary<Point, List<EntityId>> EntityIdsByTilePos,
            EntityId entityId,
            PositionComp positionComp
        )
        {
            var tilePos = positionComp.TilePos;

            if (!EntityIdsByTilePos.ContainsKey(tilePos))
            {
                Console.WriteLine( // TODO 
                    $"WARNING: {positionComp.Position.X / TILE_SIZE}, {positionComp.Position.Y / TILE_SIZE} out of bounds {tilePos}"
                );
                return Enumerable.Empty<EntityId>().ToList();
            }

            var tilesPositions = TilesNeighbours[tilePos]
                .Select(tileId => TilePositionsByTileId[tileId])
                .ToList();

            tilesPositions.Add(tilePos);

            return tilesPositions
                .SelectMany(tilePos => EntityIdsByTilePos[tilePos])
                .Distinct()
                .Where(id => id != entityId)
                .ToList();
        }

        private void SetTileNeighbours(Point tilePos) =>
            NeighbourTilePositions
                .Select(neighbourTilePos => new Point(
                    tilePos.X + neighbourTilePos.X,
                    tilePos.Y + neighbourTilePos.Y
                ))
                .Where(TileIdByTilePos.ContainsKey)
                .Select(neighbourTilePos => TileIdByTilePos[neighbourTilePos])
                .ToList()
                .ForEach(TilesNeighbours[tilePos].Add);

        private static Size GetSize(List<string> board) =>
            new Size(board[0].Length, board.Count);
    }
}
