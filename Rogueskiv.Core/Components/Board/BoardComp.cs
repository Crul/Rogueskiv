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

        public List<string> Board { get; set; }

        private readonly Dictionary<(int x, int y), List<EntityId>> WallsByTiles;
        // TODO ??? private readonly Dictionary<(int x, int y), List<EntityId>> TilesNeighbours;

        public BoardComp(string boardData)
        {
            Board = boardData.Split(Environment.NewLine).ToList();
            WallsByTiles = new Dictionary<(int x, int y), List<EntityId>>();
        }

        public void AddTile(int x, int y) =>
            WallsByTiles[(x, y)] = new List<EntityId>();

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

        public List<EntityId> GetWallsIdsNear(PositionComp position) =>
            WallsByTiles[(
                (int)Math.Floor(position.X / TILE_SIZE),
                (int)Math.Floor(position.Y / TILE_SIZE)
            )];
    }
}
