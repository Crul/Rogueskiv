using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.Components.Walls;
using Rogueskiv.MapGeneration;
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

        private MapGenerationParams MapGenerationParams;
        private string BoardData;
        private BoardComp BoardComp;

        public BoardSys(string boardData) =>
             BoardData = boardData;

        public BoardSys(MapGenerationParams mapGenerationParams) =>
             MapGenerationParams = mapGenerationParams;

        public override bool Init(Game game)
        {
            if (string.IsNullOrEmpty(BoardData))
            {
                while (string.IsNullOrEmpty(BoardData))
                    BoardData = MapGenerator.GenerateMap(MapGenerationParams);

                Console.WriteLine(BoardData);
            }

            BoardComp = game
                .Entities
                .GetWithComponent<BoardComp>()
                .Single()
                .GetComponent<BoardComp>();

            BoardComp.Board = BoardData
                .Split(Environment.NewLine)
                .Where(line => !string.IsNullOrEmpty(line))
                .ToList();

            var board = BoardComp.Board;
            (var width, var height) = GetSize(board);

            AddTiles(game, board, width, height);
            // TODO fix empty cell needed around the board to make all walls
            AddUpWalls(game, board, width, height);
            AddDownWalls(game, board, width, height);
            AddLeftWalls(game, board, width, height);
            AddRightWalls(game, board, width, height);

            return base.Init(game);
        }

        public override void Update(EntityList entities, IEnumerable<int> controls) =>
            entities
                .GetWithComponent<MovementComp>()
                .ForEach(entity => BoardComp.UpdateEntity(
                    entity.Id,
                    entity.GetComponent<CurrentPositionComp>(),
                    entity.GetComponent<LastPositionComp>()
                )
            );

        public static (int width, int height) GetSize(List<string> board) =>
            (board[0].Length, board.Count);

        #region Tiles
        private void AddTiles(Game game, List<string> board, int width, int height)
        {
            ForAllCells(width, height, (x, y) =>
            {
                if (!IsTile(board, x, y))
                    return;

                var tile = game.AddEntity(new TileComp(x, y));
                BoardComp.AddTile(x, y, tile.Id);
            });

            BoardComp.SetTilesNeighbours();
        }

        public static bool IsTile(List<string> board, int x, int y) =>
            board[y].Substring(x, 1) == TILE_CHAR;

        #endregion

        #region Walls
        private void AddUpWalls(Game game, List<string> board, int width, int height) =>
            AddWalls(
                game,
                height, width,
                isWall: (y, x) => IsTile(board, x, y) && !IsTile(board, x, y + 1),
                initWall: (y, x) => (x, y, 1),
                createComponent: wall => new UpWallComp(wall.x, wall.y, wall.size),
                maringIndex1: 1
            );

        private void AddDownWalls(Game game, List<string> board, int width, int height) =>
            AddWalls(
                game, height, width,
                isWall: (y, x) => !IsTile(board, x, y - 1) && IsTile(board, x, y),
                initWall: (y, x) => (x, y, 1),
                createComponent: wall => new DownWallComp(wall.x, wall.y, wall.size),
                initIndex1: 1
            );

        private void AddLeftWalls(Game game, List<string> board, int width, int height) =>
            AddWalls(
                game, width, height,
                isWall: (x, y) => !IsTile(board, x + 1, y) && IsTile(board, x, y),
                initWall: (x, y) => (x, y, 1),
                createComponent: wall => new LeftWallComp(wall.x, wall.y, wall.size),
                maringIndex1: 1
            );

        private void AddRightWalls(Game game, List<string> board, int width, int height) =>
            AddWalls(
                game, width, height,
                isWall: (x, y) => !IsTile(board, x - 1, y) && IsTile(board, x, y),
                initWall: (x, y) => (x, y, 1),
                createComponent: wall => new RightWallComp(wall.x, wall.y, wall.size),
                initIndex1: 1
            );

        private void AddWalls(
            Game game,
            int lengthIndex1,
            int lengthIndex2,
            Func<int, int, bool> isWall,
            Func<int, int, (int x, int y, int size)> initWall,
            Func<(int x, int y, int size), IWallComp> createComponent,
            int initIndex1 = 0,
            int maringIndex1 = 0
        )
        {
            (int x, int y, int size)? tmpWall;

            for (var i1 = initIndex1; i1 < lengthIndex1 - maringIndex1; i1++)
            {
                tmpWall = null;
                for (var i2 = 0; i2 < lengthIndex2; i2++)
                {
                    if (isWall(i1, i2))
                    {
                        tmpWall = tmpWall.HasValue
                            ? ExtendWall(tmpWall.Value)
                            : initWall(i1, i2);
                    }
                    else if (tmpWall.HasValue)
                    {
                        AddWall(game, createComponent(tmpWall.Value));
                        tmpWall = null;
                    }
                }

                if (tmpWall.HasValue)
                    AddWall(game, createComponent(tmpWall.Value));
            }
        }

        private void AddWall(Game game, IWallComp wallComp)
        {
            var wall = game.AddEntity(wallComp);
            BoardComp.AddWall(wall, wallComp);
        }

        private static (int x, int y, int size)? ExtendWall((int x, int y, int size) wall) =>
            (wall.x, wall.y, wall.size + 1);

        #endregion

        public static void ForAllCells(int width, int height, Action<int, int> action)
        {
            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                    action(x, y);
        }
    }
}
