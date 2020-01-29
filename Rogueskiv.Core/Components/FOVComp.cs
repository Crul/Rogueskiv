using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.Systems;
using Seedwork.Core.Components;
using Seedwork.Crosscutting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Rogueskiv.Core.Components
{
    public class FOVComp : IComponent
    {
        private FOVRecurse FOVRecurse;
        private Size DoubleBoardSize;
        private TileFOVInfo[,] TileFOVInfos;
        private readonly List<(int x, int y)> indexList =
            new List<(int, int)> { (0, 0), (0, 1), (1, 0), (1, 1) };

        public void Init(BoardComp boardComp, PlayerComp playerComp)
        {
            DoubleBoardSize = boardComp.BoardSize.Multiply(2);
            TileFOVInfos = new TileFOVInfo[DoubleBoardSize.Width, DoubleBoardSize.Height];
            ForAllSubTiles((x, y) => TileFOVInfos[x, y] = new TileFOVInfo(x, y));

            FOVRecurse = new FOVRecurse(boardComp.BoardSize.Width, boardComp.BoardSize.Height);
            SetVisualRange(playerComp);
            BoardSys.ForAllTiles(boardComp.BoardSize, tilePos => // TODO move ForAllTiles & IsTile to BoardComp ?
                FOVRecurse.Point_Set(tilePos.X, tilePos.Y, !BoardSys.IsTile(boardComp.Board, tilePos) ? 1 : 0));
        }

        public void SetVisualRange(PlayerComp playerComp)
            => FOVRecurse.VisualRange = (int)playerComp.VisualRange;

        public void RevealAll(BoardComp boardComp) =>
            ForAllSubTiles((x, y) =>
            {
                var tilePos = new Point(x, y).Divide(2);
                var isTile = IsTile(boardComp, tilePos);
                if (isTile)
                {
                    TileFOVInfos[x, y].Reveal();
                    return;
                }

                var isWall = boardComp.WallsByTiles.ContainsKey(tilePos);
                if (!isWall)
                    return;

                var reveal = false;
                var isLeftSubtile = x % 2 == 0;
                var isTopSubtile = y % 2 == 0;
                if (isLeftSubtile && isTopSubtile)        // TOP LEFT
                    reveal = IsTile(boardComp, tilePos.Substract(x: 1))
                        || IsTile(boardComp, tilePos.Substract(y: 1))
                        || IsTile(boardComp, tilePos.Substract(x: 1, y: 1));

                else if (!isLeftSubtile && isTopSubtile)  // TOP RIGHT
                    reveal = IsTile(boardComp, tilePos.Add(x: 1))
                        || IsTile(boardComp, tilePos.Substract(y: 1))
                        || IsTile(boardComp, tilePos.Add(x: 1, y: -1));

                else if (!isLeftSubtile && !isTopSubtile) // BOTTOM RIGHT
                    reveal = IsTile(boardComp, tilePos.Add(x: 1))
                        || IsTile(boardComp, tilePos.Add(y: 1))
                        || IsTile(boardComp, tilePos.Add(x: 1, y: 1));

                else if (isLeftSubtile && !isTopSubtile)  // BOTTOM LEFT
                    reveal = IsTile(boardComp, tilePos.Substract(x: 1))
                        || IsTile(boardComp, tilePos.Add(y: 1))
                        || IsTile(boardComp, tilePos.Add(x: -1, y: 1));

                if (reveal)
                    TileFOVInfos[x, y].Reveal();
            });

        public void SetPlayerPos(PlayerComp playerComp, IPositionComp playerPosComp)
        {
            Reset();

            FOVRecurse.SetPlayerPos(playerPosComp.TilePos.X, playerPosComp.TilePos.Y);
            ForAllSubTiles(tileFOVInfo =>
            {
                tileFOVInfo.Visible = FOVRecurse.IsPointVisible(tileFOVInfo.TileFOVPos);
                tileFOVInfo.DistanceFactor =
                    Distance.Get(tileFOVInfo.Position, playerPosComp.Position)
                    / (BoardComp.TILE_SIZE * (playerComp.VisualRange + 1));
            });
        }

        public bool IsVisibleByPlayer(IPositionComp positionComp) =>
            GetFOVTiles(positionComp.TilePos)
                .Any(tileFOVinfo => tileFOVinfo.VisibleByPlayer);

        public bool HasBeenSeenOrRevealed(IPositionComp positionComp) =>
            GetFOVTiles(positionComp.TilePos)
                .Any(tileFOVinfo => tileFOVinfo.HasBeenSeenOrRevealed(positionComp.AllowRevealedByMap));

        private void Reset() =>
            ForAllSubTiles((x, y) => Reset(TileFOVInfos[x, y]));

        private static void Reset(TileFOVInfo tileFOVInfo)
        {
            tileFOVInfo.VisibleByPlayer = false;
            tileFOVInfo.DistanceFactor = 0;
        }

        public void ForAllSubTiles(Action<TileFOVInfo> action)
            => ForAllSubTiles((x, y) => action(TileFOVInfos[x, y]));

        private void ForAllSubTiles(Action<int, int> action)
        {
            for (var x = 0; x < DoubleBoardSize.Width; x++)
                for (var y = 0; y < DoubleBoardSize.Height; y++)
                    action(x, y);
        }

        private List<TileFOVInfo> GetFOVTiles(Point tilePos)
        {
            var fovTilePos = tilePos.Multiply(2).ToPoint();

            return indexList
                .Select(index => TileFOVInfos[fovTilePos.X + index.x, fovTilePos.Y + index.y])
                .ToList();
        }

        private static bool IsTile(BoardComp boardComp, Point tilePos) =>
            boardComp.TileIdByTilePos.ContainsKey(tilePos);
    }
}
