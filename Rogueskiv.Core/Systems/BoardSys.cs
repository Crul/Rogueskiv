using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Walls;
using Seedwork.Core;
using Seedwork.Core.Components;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    public class BoardSys : BaseSystem
    {
        public override bool Init(Game game)
        {
            var boardEntity = game
                .Entities
                .GetWithComponent<BoardComp>()
                .Single();

            var board = boardEntity.GetComponent<BoardComp>().Board;
            var width = board[0].Length;
            var height = board.Count;

            AddTiles(game, board, width, height);
            // TODO fix empty cell needed around the board to make all walls
            AddUpWalls(game, board, width, height);
            AddDownWalls(game, board, width, height);
            AddLeftWalls(game, board, width, height);
            AddRightWalls(game, board, width, height);

            game.Entities.Remove(boardEntity);

            return false;
        }

        private static void AddUpWalls(Game game, List<string> board, int width, int height) =>
            AddWalls(
                game, width, height,
                isWall: (x, y) => IsTile(board, x, y - 1) && !IsTile(board, x, y),
                createComponent: wall => new UpWallComp(wall.x, wall.y, wall.size),
                initHeight: 1
            );

        private static void AddDownWalls(Game game, List<string> board, int width, int height) =>
            AddWalls(
                game, width, height,
                isWall: (x, y) => !IsTile(board, x, y - 1) && IsTile(board, x, y),
                createComponent: wall => new DownWallComp(wall.x, wall.y, wall.size),
                initHeight: 1
            );

        private static void AddLeftWalls(Game game, List<string> board, int width, int height) =>
            AddWalls(
                game, width, height,
                isWall: (x, y) => !IsTile(board, x, y) && IsTile(board, x - 1, y),
                createComponent: wall => new LeftWallComp(wall.x, wall.y, wall.size),
                initWidth: 1
            );

        private static void AddRightWalls(Game game, List<string> board, int width, int height) =>
            AddWalls(
                game, width, height,
                isWall: (x, y) => !IsTile(board, x - 1, y) && IsTile(board, x, y),
                createComponent: wall => new RightWallComp(wall.x, wall.y, wall.size),
                initWidth: 1
            );

        private static void AddWalls(
            Game game,
            int width,
            int height,
            Func<int, int, bool> isWall,
            Func<(int x, int y, int size), IComponent> createComponent,
            int initWidth = 0,
            int initHeight = 0
        )
        {
            (int x, int y, int size)? tmpWall = null;

            for (var y = initHeight; y < height; y++)
            {
                for (var x = initWidth; x < width; x++)
                {
                    if (isWall(x, y))
                    {
                        if (tmpWall.HasValue)
                            tmpWall = ExtendWall(tmpWall.Value);

                        else
                            tmpWall = (x, y, 1);
                    }
                    else if (tmpWall.HasValue)
                    {
                        game.AddEntity(createComponent(tmpWall.Value));
                        tmpWall = null;
                    }
                }

                if (tmpWall.HasValue)
                {
                    game.AddEntity(createComponent(tmpWall.Value));
                    tmpWall = null;
                }
            }
        }

        private static void AddTiles(Game game, List<string> board, int width, int height)
        {
            ForAllCells(width, height, (x, y) =>
            {
                if (IsTile(board, x, y))
                    game.AddEntity(new TileComp(x, y));
            });
        }

        private static bool IsTile(List<string> board, int x, int y) =>
            board[y].Substring(x, 1) == "T";

        private static void ForAllCells(int width, int height, Action<int, int> action)
        {
            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                    action(x, y);
        }

        private static (int x, int y, int size)? ExtendWall((int x, int y, int size) wall) =>
            (wall.x, wall.y, wall.size + 1);

        public override void Update(List<IEntity> entities, IEnumerable<int> controls) { }
    }
}
