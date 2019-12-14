using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Walls;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    public class BoardSys : BaseSystem
    {
        private const string TILE_CHAR = "T";

        public override bool Init(Game game)
        {
            var boardComp = game
                .Entities
                .GetWithComponent<BoardComp>()
                .Single()
                .GetComponent<BoardComp>();

            var board = boardComp.Board;
            var width = board[0].Length;
            var height = board.Count;

            AddTiles(game, boardComp, board, width, height);
            // TODO fix empty cell needed around the board to make all walls
            AddUpWalls(game, boardComp, board, width, height);
            AddDownWalls(game, boardComp, board, width, height);
            AddLeftWalls(game, boardComp, board, width, height);
            AddRightWalls(game, boardComp, board, width, height);

            return false;
        }

        #region Tiles
        private static void AddTiles(
            Game game, BoardComp boardComp, List<string> board, int width, int height
        )
        {
            ForAllCells(width, height, (x, y) =>
            {
                if (!IsTile(board, x, y))
                    return;

                var tile = game.AddEntity(new TileComp(x, y));
                boardComp.AddTile(x, y, tile.Id);
            });

            boardComp.SetTilesNeighbours();
        }

        private static bool IsTile(List<string> board, int x, int y) =>
            board[y].Substring(x, 1) == TILE_CHAR;

        #endregion

        #region Walls
        private static void AddUpWalls(
            Game game, BoardComp boardComp, List<string> board, int width, int height
        ) =>
            AddWalls(
                game, boardComp,
                height, width,
                isWall: (y, x) => IsTile(board, x, y - 1) && !IsTile(board, x, y),
                initWall: (y, x) => (x, y, 1),
                createComponent: wall => new UpWallComp(wall.x, wall.y, wall.size),
                initIndex1: 1
            );

        private static void AddDownWalls(
            Game game, BoardComp boardComp, List<string> board, int width, int height
        ) =>
            AddWalls(
                game, boardComp,
                height, width,
                isWall: (y, x) => !IsTile(board, x, y - 1) && IsTile(board, x, y),
                initWall: (y, x) => (x, y, 1),
                createComponent: wall => new DownWallComp(wall.x, wall.y, wall.size),
                initIndex1: 1
            );

        private static void AddLeftWalls(
            Game game, BoardComp boardComp, List<string> board, int width, int height
        ) =>
            AddWalls(
                game, boardComp,
                width, height,
                isWall: (x, y) => !IsTile(board, x, y) && IsTile(board, x - 1, y),
                initWall: (x, y) => (x, y, 1),
                createComponent: wall => new LeftWallComp(wall.x, wall.y, wall.size),
                initIndex1: 1
            );

        private static void AddRightWalls(
            Game game, BoardComp boardComp, List<string> board, int width, int height
        ) =>
            AddWalls(
                game, boardComp,
                width, height,
                isWall: (x, y) => !IsTile(board, x - 1, y) && IsTile(board, x, y),
                initWall: (x, y) => (x, y, 1),
                createComponent: wall => new RightWallComp(wall.x, wall.y, wall.size),
                initIndex1: 1
            );

        private static void AddWalls(
            Game game,
            BoardComp boardComp,
            int lengthIndex1,
            int lengthIndex2,
            Func<int, int, bool> isWall,
            Func<int, int, (int x, int y, int size)> initWall,
            Func<(int x, int y, int size), IWallComp> createComponent,
            int initIndex1 = 0,
            int initIndex2 = 0
        )
        {
            (int x, int y, int size)? tmpWall;

            for (var i1 = initIndex1; i1 < lengthIndex1; i1++)
            {
                tmpWall = null;
                for (var i2 = initIndex2; i2 < lengthIndex2; i2++)
                {
                    if (isWall(i1, i2))
                    {
                        tmpWall = tmpWall.HasValue
                            ? ExtendWall(tmpWall.Value)
                            : initWall(i1, i2);
                    }
                    else if (tmpWall.HasValue)
                    {
                        AddWall(game, boardComp, createComponent(tmpWall.Value));
                        tmpWall = null;
                    }
                }

                if (tmpWall.HasValue)
                    AddWall(game, boardComp, createComponent(tmpWall.Value));
            }
        }

        private static void AddWall(Game game, BoardComp boardComp, IWallComp wallComp)
        {
            var wall = game.AddEntity(wallComp);
            boardComp.AddWall(wall, wallComp);
        }

        private static (int x, int y, int size)? ExtendWall((int x, int y, int size) wall) =>
            (wall.x, wall.y, wall.size + 1);

        #endregion

        private static void ForAllCells(int width, int height, Action<int, int> action)
        {
            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                    action(x, y);
        }

        public override void Update(EntityList entities, IEnumerable<int> controls) { }
    }
}
