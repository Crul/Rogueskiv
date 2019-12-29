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

        private readonly IMapGenerationParams MapGenerationParams;
        private string BoardData;
        private BoardComp BoardComp;

        public BoardSys(string boardData = "") =>
             BoardData = boardData;

        public BoardSys(IMapGenerationParams mapGenerationParams) =>
             MapGenerationParams = mapGenerationParams;

        public override void Init(Game game)
        {
            while (string.IsNullOrEmpty(BoardData))
                BoardData = MapGenerator.GenerateMap(MapGenerationParams);

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

            BoardComp.SetTilesNeighbours();
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

            return tileComps;
        }

        public static bool IsTile(List<string> board, Point tilePos) =>
            board[tilePos.Y].Substring(tilePos.X, 1).ToUpper() == TILE_CHAR;

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
                game, board, size,
                isHorizontal: true,
                checkTilePosTile: (y, x) => (x, y - 1),
                createComponent: wall => new UpWallComp(wall.tilePos, wall.size),
                initIndex1: 1
            );

        private List<IWallComp> AddDownWalls(Game game, List<string> board, Size size) =>
            AddWalls(
                game, board, size,
                isHorizontal: true,
                checkTilePosTile: (y, x) => (x, y + 1),
                createComponent: wall => new DownWallComp(wall.tilePos, wall.size),
                marginIndex1: 1
            );

        private List<IWallComp> AddLeftWalls(Game game, List<string> board, Size size) =>
            AddWalls(
                game, board, size,
                isHorizontal: false,
                checkTilePosTile: (x, y) => (x - 1, y),
                createComponent: wall => new LeftWallComp(wall.tilePos, wall.size),
                initIndex1: 1
            );

        private List<IWallComp> AddRightWalls(Game game, List<string> board, Size size) =>
            AddWalls(
                game, board, size,
                isHorizontal: false,
                checkTilePosTile: (x, y) => (x + 1, y),
                createComponent: wall => new RightWallComp(wall.tilePos, wall.size),
                marginIndex1: 1
            );

        private List<IWallComp> AddWalls(
            Game game,
            List<string> board,
            Size size,
            bool isHorizontal,
            Func<int, int, (int tileX, int tileY)> checkTilePosTile,
            Func<(Point tilePos, int size), IWallComp> createComponent,
            int initIndex1 = 0,
            int marginIndex1 = 0
        )
        {
            Func<int, int, (int x, int y)> getXY;
            int lengthIndex1, lengthIndex2;
            if (isHorizontal)
            {
                getXY = (i1, i2) => (x: i2, y: i1);
                lengthIndex1 = size.Height;
                lengthIndex2 = size.Width;
            }
            else
            {
                getXY = (i1, i2) => (x: i1, y: i2);
                lengthIndex1 = size.Width;
                lengthIndex2 = size.Height;
            }

            var walls = new List<IWallComp>();
            for (var i1 = initIndex1; i1 < lengthIndex1 - marginIndex1; i1++)
            {
                (Point tilePos, int size)? wallInfo = null;
                for (var i2 = 0; i2 < lengthIndex2; i2++)
                {
                    var (wallX, wallY) = getXY(i1, i2);
                    var (tileX, tileY) = checkTilePosTile(i1, i2);
                    var wallPos = new Point(wallX, wallY);
                    var tilePos = new Point(tileX, tileY);
                    var isWall = IsTile(board, tilePos) && !IsTile(board, wallPos);

                    if (isWall)
                    {
                        if (wallInfo.HasValue)
                            wallInfo = (wallInfo.Value.tilePos, wallInfo.Value.size + 1);
                        else
                            wallInfo = ExpandTopIfNeeded((wallPos, 1), board, isHorizontal);
                    }
                    else if (wallInfo.HasValue)
                    {
                        wallInfo = ExpandBottomIfNeeded(wallInfo.Value, board, isHorizontal);
                        AddWall(game, walls, createComponent, wallInfo.Value);
                        wallInfo = null;
                    }
                }

                if (wallInfo.HasValue)
                    throw new NotImplementedException();
            }

            return walls;
        }

        private static (Point tilePos, int size) ExpandTopIfNeeded(
            (Point tilePos, int size) wallInfo,
            List<string> board,
            bool isHorizontal
        )
        {
            if (isHorizontal)
                return wallInfo;

            var tileOnTop = wallInfo.tilePos.Substract(y: 1);
            if (!IsTile(board, tileOnTop))
                return (tileOnTop, wallInfo.size + 1);

            return wallInfo;
        }

        private static (Point tilePos, int size) ExpandBottomIfNeeded(
            (Point tilePos, int size) wallInfo,
            List<string> board,
            bool isHorizontal
        )
        {
            if (isHorizontal)
                return wallInfo;

            var tileOnBottom = wallInfo.tilePos.Add(y: wallInfo.size);
            if (!IsTile(board, tileOnBottom))
                wallInfo = (wallInfo.tilePos, wallInfo.size + 1);

            return wallInfo;
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

        #endregion
    }
}
