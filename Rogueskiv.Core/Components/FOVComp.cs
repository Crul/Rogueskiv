using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.Systems;
using Seedwork.Core.Components;
using Seedwork.Crosscutting;
using System;
using System.Drawing;
using System.Linq;

namespace Rogueskiv.Core.Components
{
    public class FOVComp : IComponent
    {
        private FOVRecurse FOVRecurse;
        private Size BoardSize;
        private TileFOVInfo[,] FOVTiles;

        public bool HasMapBeenRevealed { get; set; } = false;

        public void Init(BoardComp boardComp, PlayerComp playerComp)
        {
            BoardSize = boardComp.BoardSize;

            FOVTiles = new TileFOVInfo[BoardSize.Width, BoardSize.Height];
            ForAllTiles((x, y) => FOVTiles[x, y] = new TileFOVInfo());

            FOVRecurse = new FOVRecurse(BoardSize.Width, BoardSize.Height, playerComp.VisualRange);

            BoardSys.ForAllTiles(BoardSize, tilePos =>
                FOVRecurse.Point_Set(tilePos.X, tilePos.Y, !BoardSys.IsTile(boardComp.Board, tilePos) ? 1 : 0));
        }

        public TileFOVInfo GetTileFOVInfo(int x, int y) => FOVTiles[x, y];

        public void SetPlayerPos(PositionComp positionComp) =>
            FOVRecurse.SetPlayerPos(positionComp.TilePos.X, positionComp.TilePos.Y);

        public bool IsVisible(PositionComp positionComp) =>
            FOVRecurse.VisiblePoints.Any(vp => vp == positionComp.TilePos);

        public void Reset() =>
            ForAllTiles((x, y) => Reset(FOVTiles[x, y]));

        private static void Reset(TileFOVInfo tileFOVInfo)
        {
            tileFOVInfo.CoveredByFOV = false;
            tileFOVInfo.VisibleByPlayer = false;
            tileFOVInfo.DistanceFromPlayer = 0;
        }

        private void ForAllTiles(Action<int, int> action)
        {
            for (var x = 0; x < BoardSize.Width; x++)
                for (var y = 0; y < BoardSize.Height; y++)
                    action(x, y);
        }
    }
}
