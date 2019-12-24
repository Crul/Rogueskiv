using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.Components.Walls;
using Rogueskiv.MapGeneration;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using Seedwork.Crosscutting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    public class BoardSys : BaseSystem
    {
        private const string TILE_CHAR = "T";

        private readonly MapGenerationParams MapGenerationParams;
        private string BoardData;
        private BoardComp BoardComp;

        public BoardSys(string boardData = "") =>
             BoardData = boardData;

        public BoardSys(MapGenerationParams mapGenerationParams) =>
             MapGenerationParams = mapGenerationParams;

        public override void Init(Game game)
        {
            if (string.IsNullOrEmpty(BoardData))
            {
                while (string.IsNullOrEmpty(BoardData))
                    BoardData = MapGenerator.GenerateMap(MapGenerationParams);

                Console.WriteLine(BoardData);
            }

            BoardComp = game.Entities.GetSingleComponent<BoardComp>();

            BoardComp.Board = BoardData
                .Split(Environment.NewLine)
                .Where(line => !string.IsNullOrEmpty(line))
                .ToList();

            var board = BoardComp.Board;
            var size = BoardComp.BoardSize;

            var tileComps = AddTiles(game, board, size);

            // TODO fix empty tile needed around the board to make all walls
            AddLeftWalls(game, board, size);
            AddRightWalls(game, board, size);
            AddUpWalls(game, board, size);
            AddDownWalls(game, board, size);

            SetCornerTiles(game, size);
        }

        public override void Update(EntityList entities, List<int> controls) =>
            entities
                .GetWithComponent<MovementComp>()
                .ForEach(entity => BoardComp.UpdateEntity(
                    entity.Id,
                    entity.GetComponent<CurrentPositionComp>(),
                    entity.GetComponent<LastPositionComp>()
                )
            );

        #region Tiles
        private List<TileComp> AddTiles(Game game, List<string> board, Size size)
        {
            var tileComps = new List<TileComp>();
            ForAllTiles(size, tilePos =>
            {
                if (!IsTile(board, tilePos))
                    return;

                var tileComp = new TileComp(tilePos);
                var tile = game.AddEntity(tileComp);
                tileComps.Add(tileComp);
                BoardComp.AddTile(tilePos, tile.Id);
            });

            BoardComp.SetTilesNeighbours();

            return tileComps;
        }

        private void SetCornerTiles(Game game, Size size) =>
            ForAllTiles(size, tilePos => SetCornerTiles(game, tilePos));

        private void SetCornerTiles(Game game, Point tilePos)
        {
            var tile = TryGetTile(game, tilePos);
            if (tile == null)
                return;

            var tileUp = TryGetTile(game, tilePos.Add(y: -1));
            var tileDown = TryGetTile(game, tilePos.Add(y: 1));
            var tileLeft = TryGetTile(game, tilePos.Add(x: -1));
            var tileRight = TryGetTile(game, tilePos.Add(x: 1));

            if (TileHasWall(tileUp, TileWallFlags.Right) && TileHasWall(tileLeft, TileWallFlags.Down))
                tile.AddWall(TileWallFlags.CornerDownRight);

            if (TileHasWall(tileUp, TileWallFlags.Left) && TileHasWall(tileRight, TileWallFlags.Down))
                tile.AddWall(TileWallFlags.CornerDownLeft);

            if (TileHasWall(tileDown, TileWallFlags.Right) && TileHasWall(tileLeft, TileWallFlags.Up))
                tile.AddWall(TileWallFlags.CornerUpRight);

            if (TileHasWall(tileDown, TileWallFlags.Left) && TileHasWall(tileRight, TileWallFlags.Up))
                tile.AddWall(TileWallFlags.CornerUpLeft);
        }

        private TileComp TryGetTile(Game game, Point tilePos)
        {
            if (!BoardComp.TileIdByTilePos.ContainsKey(tilePos))
                return null;

            var tileId = BoardComp.TileIdByTilePos[tilePos];

            return game.Entities[tileId].GetComponent<TileComp>();
        }

        public static bool IsTile(List<string> board, Point tilePos) =>
            board[tilePos.Y].Substring(tilePos.X, 1).ToUpper() == TILE_CHAR;

        private static bool TileHasWall(TileComp tileComp, TileWallFlags wallFlag) =>
            tileComp != null && ((tileComp.WallFlags & wallFlag) != 0);

        public static void ForAllTiles(Size size, Action<Point> action)
        {
            for (var y = 0; y < size.Height; y++)
                for (var x = 0; x < size.Width; x++)
                    action(new Point(x, y));
        }
        #endregion

        #region Walls
        private List<IWallComp> AddUpWalls(Game game, List<string> board, Size size) =>
            AddWalls(
                game, board,
                getXY: (y, x) => (x, y),
                checkWallPosTile: (y, x) => (x, y + 1),
                createComponent: wall => new UpWallComp(wall.tilePos, wall.size),
                tileWallFlag: TileWallFlags.Up,
                lengthIndex1: size.Height,
                lengthIndex2: size.Width,
                marginIndex1: 1
            );

        private List<IWallComp> AddDownWalls(Game game, List<string> board, Size size) =>
            AddWalls(
                game, board,
                getXY: (y, x) => (x, y),
                checkWallPosTile: (y, x) => (x, y - 1),
                createComponent: wall => new DownWallComp(wall.tilePos, wall.size),
                tileWallFlag: TileWallFlags.Down,
                lengthIndex1: size.Height,
                lengthIndex2: size.Width,
                initIndex1: 1
            );

        private List<IWallComp> AddLeftWalls(Game game, List<string> board, Size size) =>
            AddWalls(
                game, board,
                getXY: (x, y) => (x, y),
                checkWallPosTile: (x, y) => (x + 1, y),
                createComponent: wall => new LeftWallComp(wall.tilePos, wall.size),
                tileWallFlag: TileWallFlags.Left,
                lengthIndex1: size.Width,
                lengthIndex2: size.Height,
                marginIndex1: 1
            );

        private List<IWallComp> AddRightWalls(Game game, List<string> board, Size size) =>
            AddWalls(
                game, board,
                getXY: (x, y) => (x, y),
                checkWallPosTile: (x, y) => (x - 1, y),
                createComponent: wall => new RightWallComp(wall.tilePos, wall.size),
                tileWallFlag: TileWallFlags.Right,
                lengthIndex1: size.Width,
                lengthIndex2: size.Height,
                initIndex1: 1
            );

        private List<IWallComp> AddWalls(
            Game game,
            List<string> board,
            Func<int, int, (int tileX, int tileY)> getXY,
            Func<int, int, (int tileX, int tileY)> checkWallPosTile,
            Func<(Point tilePos, int size), IWallComp> createComponent,
            TileWallFlags tileWallFlag,
            int lengthIndex1,
            int lengthIndex2,
            int initIndex1 = 0,
            int marginIndex1 = 0
        )
        {
            var walls = new List<IWallComp>();
            for (var i1 = initIndex1; i1 < lengthIndex1 - marginIndex1; i1++)
            {
                (Point tilePos, int size)? tmpWall = null;
                for (var i2 = 0; i2 < lengthIndex2; i2++)
                {
                    var (tileX, tileY) = getXY(i1, i2);
                    var (wallX, wallY) = checkWallPosTile(i1, i2);
                    var tilePos = new Point(tileX, tileY);
                    var isWall =
                        IsTile(board, tilePos)
                        && !IsTile(board, new Point(wallX, wallY));

                    if (isWall)
                    {
                        var tileId = BoardComp.TileIdByTilePos[tilePos];
                        var tileComp = game.Entities[tileId].GetComponent<TileComp>();
                        tileComp.AddWall(tileWallFlag);

                        tmpWall = tmpWall.HasValue
                            ? ExtendWall(tmpWall.Value)
                            : (tilePos, 1);
                    }
                    else if (tmpWall.HasValue)
                    {
                        AddWall(game, walls, createComponent, tmpWall.Value);
                        tmpWall = null;
                    }
                }

                if (tmpWall.HasValue)
                    AddWall(game, walls, createComponent, tmpWall.Value);
            }

            return walls;
        }

        private void AddWall(
            Game game,
            List<IWallComp> walls,
            Func<(Point tilePos, int size), IWallComp> createComponent,
            (Point tilePos, int size) wallInfo
        )
        {
            var wallComp = createComponent((wallInfo.tilePos, wallInfo.size));
            walls.Add(wallComp);

            var wallEntity = game.AddEntity(wallComp);
            BoardComp.AddWall(wallEntity, wallComp);
        }

        private static (Point tilePos, int size)? ExtendWall((Point tilePos, int size) wall) =>
            (wall.tilePos, wall.size + 1);

        #endregion
    }
}
