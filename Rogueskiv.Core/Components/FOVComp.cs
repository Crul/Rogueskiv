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
            ForAllTiles((x, y) => TileFOVInfos[x, y] = new TileFOVInfo(x, y));

            FOVRecurse = new FOVRecurse(boardComp.BoardSize.Width, boardComp.BoardSize.Height);
            SetVisualRange(playerComp);
            BoardSys.ForAllTiles(boardComp.BoardSize, tilePos => // TODO move ForAllTiles & IsTile to BoardComp ?
                FOVRecurse.Point_Set(tilePos.X, tilePos.Y, !BoardSys.IsTile(boardComp.Board, tilePos) ? 1 : 0));
        }

        public void SetVisualRange(PlayerComp playerComp)
            => FOVRecurse.VisualRange = playerComp.VisualRange;

        public void RevealAll() =>
            ForAllTiles((x, y) => TileFOVInfos[x, y].Reveal());

        public void SetPlayerPos(PlayerComp playerComp, IPositionComp playerPosComp)
        {
            Reset();

            FOVRecurse.SetPlayerPos(playerPosComp.TilePos.X, playerPosComp.TilePos.Y);
            ForAllTiles(tileFOVInfo =>
            {
                tileFOVInfo.Visible = FOVRecurse.IsPointVisible(tileFOVInfo.TileFOVPos);
                tileFOVInfo.DistanceFactor =
                    Distance.Get(tileFOVInfo.Position, playerPosComp.Position)
                    / (BoardComp.TILE_SIZE * playerComp.VisualRange);
            });
        }

        public bool IsVisibleByPlayer(IPositionComp positionComp) =>
            GetFOVTiles(positionComp.TilePos)
                .Any(tileFOVinfo => tileFOVinfo.VisibleByPlayer);

        public bool IsVisible(IPositionComp positionComp) =>
            GetFOVTiles(positionComp.TilePos)
                .Any(tileFOVinfo => tileFOVinfo.Visible);

        private void Reset() =>
            ForAllTiles((x, y) => Reset(TileFOVInfos[x, y]));

        private static void Reset(TileFOVInfo tileFOVInfo)
        {
            tileFOVInfo.VisibleByPlayer = false;
            tileFOVInfo.DistanceFactor = 0;
        }

        public void ForAllTiles(Action<TileFOVInfo> action)
            => ForAllTiles((x, y) => action(TileFOVInfos[x, y]));

        private void ForAllTiles(Action<int, int> action)
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
    }
}
