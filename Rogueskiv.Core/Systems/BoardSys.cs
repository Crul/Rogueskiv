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
using System.Drawing;
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
            var size = BoardComp.BoardSize;

            AddTiles(game, board, size);

            // TODO fix empty tile needed around the board to make all walls
            AddLeftWalls(game, board, size);
            AddRightWalls(game, board, size);
            var upWalls = AddUpWalls(game, board, size);
            var downWalls = AddDownWalls(game, board, size);

            SetWallTips(game, upWalls, downWalls);

            return base.Init(game);
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
        private void AddTiles(Game game, List<string> board, Size size)
        {
            ForAllTiles(size, tilePos =>
            {
                if (!IsTile(board, tilePos))
                    return;

                var tile = game.AddEntity(new TileComp(tilePos));
                BoardComp.AddTile(tilePos, tile.Id);
            });

            BoardComp.SetTilesNeighbours();
        }

        public static bool IsTile(List<string> board, Point tilePos) =>
            board[tilePos.Y].Substring(tilePos.X, 1).ToUpper() == TILE_CHAR;

        #endregion

        #region Walls
        private List<IWallComp> AddUpWalls(Game game, List<string> board, Size size) =>
            AddWalls(
                game,
                size.Height, size.Width,
                isWall: (y, x) => IsTile(board, new Point(x, y)) && !IsTile(board, new Point(x, y + 1)),
                initWall: (y, x) => (x, y, 1),
                createWallTile: (y, x) => new WallTile(new Point(x, y)),
                createComponent: wall => new UpWallComp(new Point(wall.x, wall.y), wall.size, wall.tiles),
                maringIndex1: 1
            );

        private List<IWallComp> AddDownWalls(Game game, List<string> board, Size size) =>
            AddWalls(
                game,
                size.Height, size.Width,
                isWall: (y, x) => !IsTile(board, new Point(x, y - 1)) && IsTile(board, new Point(x, y)),
                initWall: (y, x) => (x, y, 1),
                createWallTile: (y, x) => new WallTile(new Point(x, y)),
                createComponent: wall => new DownWallComp(new Point(wall.x, wall.y), wall.size, wall.tiles),
                initIndex1: 1
            );

        private List<IWallComp> AddLeftWalls(Game game, List<string> board, Size size) =>
            AddWalls(
                game,
                size.Width, size.Height,
                isWall: (x, y) => !IsTile(board, new Point(x + 1, y)) && IsTile(board, new Point(x, y)),
                initWall: (x, y) => (x, y, 1),
                createWallTile: (x, y) => new WallTile(new Point(x, y)),
                createComponent: wall => new LeftWallComp(new Point(wall.x, wall.y), wall.size, wall.tiles),
                maringIndex1: 1
            );

        private List<IWallComp> AddRightWalls(Game game, List<string> board, Size size) =>
            AddWalls(
                game,
                size.Width, size.Height,
                isWall: (x, y) => !IsTile(board, new Point(x - 1, y)) && IsTile(board, new Point(x, y)),
                initWall: (x, y) => (x, y, 1),
                createWallTile: (x, y) => new WallTile(new Point(x, y)),
                createComponent: wall => new RightWallComp(new Point(wall.x, wall.y), wall.size, wall.tiles),
                initIndex1: 1
            );

        private List<IWallComp> AddWalls(
            Game game,
            int lengthIndex1,
            int lengthIndex2,
            Func<int, int, bool> isWall,
            Func<int, int, (int x, int y, int size)> initWall,
            Func<int, int, WallTile> createWallTile,
            Func<(int x, int y, int size, List<WallTile> tiles), IWallComp> createComponent,
            int initIndex1 = 0,
            int maringIndex1 = 0
        )
        {
            var walls = new List<IWallComp>();
            (int x, int y, int size)? tmpWall;
            var wallTiles = new List<WallTile>();

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

                        wallTiles.Add(createWallTile(i1, i2));
                    }
                    else if (tmpWall.HasValue)
                    {
                        AddWall(game, walls, createComponent, tmpWall.Value, wallTiles);
                        tmpWall = null;
                        wallTiles = new List<WallTile>();
                    }
                }

                if (tmpWall.HasValue)
                    AddWall(game, walls, createComponent, tmpWall.Value, wallTiles);
            }

            return walls;
        }

        private void AddWall(
            Game game,
            List<IWallComp> walls,
            Func<(int x, int y, int size, List<WallTile> tiles), IWallComp> createComponent,
            (int x, int y, int size) wallInfo,
            List<WallTile> wallTiles
        )
        {
            var wallComp = createComponent((wallInfo.x, wallInfo.y, wallInfo.size, wallTiles));
            walls.Add(wallComp);

            var wallEntity = game.AddEntity(wallComp);
            BoardComp.AddWall(wallEntity, wallComp);
        }

        private static (int x, int y, int size)? ExtendWall((int x, int y, int size) wall) =>
            (wall.x, wall.y, wall.size + 1);

        private void SetWallTips(Game game, List<IWallComp> upWalls, List<IWallComp> downWalls)
        {
            upWalls.ForEach(upWall =>
            {
                var initialTile = upWall.Tiles.First();
                initialTile.InitialTip = GetConvexity(
                    game,
                    targetTile: new Point(
                        initialTile.TilePos.X - 1,
                        initialTile.TilePos.Y + 1
                    ),
                    facingTarget: WallFacingDirections.LEFT
                );

                var finalTile = upWall.Tiles.Last();
                finalTile.FinalTip = GetConvexity(
                    game,
                    targetTile: new Point(
                        finalTile.TilePos.X + 1,
                        finalTile.TilePos.Y + 1
                    ),
                    facingTarget: WallFacingDirections.RIGHT
                );
            });

            downWalls.ForEach(downWall =>
            {
                var initialTile = downWall.Tiles.First();
                initialTile.InitialTip = GetConvexity(
                    game,
                    targetTile: new Point(
                        initialTile.TilePos.X - 1,
                        initialTile.TilePos.Y - 1
                    ),
                    facingTarget: WallFacingDirections.LEFT
                );

                var finalTile = downWall.Tiles.Last();
                finalTile.FinalTip = GetConvexity(
                    game,
                    targetTile: new Point(
                        finalTile.TilePos.X + 1,
                        finalTile.TilePos.Y - 1
                    ),
                    facingTarget: WallFacingDirections.RIGHT
                );
            });
        }

        private WallTipTypes GetConvexity(Game game, Point targetTile, WallFacingDirections facingTarget)
        {
            var isConvexe =
                BoardComp.WallsByTiles.ContainsKey(targetTile)
                && BoardComp
                    .WallsByTiles[targetTile]
                    .Any(wallId =>
                        game.Entities[wallId].GetComponent<IWallComp>().Facing == facingTarget
                    );

            return isConvexe ? WallTipTypes.CONVEXE : WallTipTypes.CONCAVE;
        }

        #endregion

        public static void ForAllTiles(Size size, Action<Point> action)
        {
            for (var y = 0; y < size.Height; y++)
                for (var x = 0; x < size.Width; x++)
                    action(new Point(x, y));
        }
    }
}
