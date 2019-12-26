using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.Systems;
using Seedwork.Core.Components;
using Seedwork.Crosscutting;
using System;
using System.Drawing;

namespace Rogueskiv.Core.Components
{
    public class FOVComp : IComponent
    {
        private FOVRecurse FOVRecurse;
        private Size BoardSize;
        private TileFOVInfo[,] FOVTiles;

        public void Init(BoardComp boardComp, PlayerComp playerComp)
        {
            BoardSize = boardComp.BoardSize;

            FOVTiles = new TileFOVInfo[BoardSize.Width, BoardSize.Height];
            ForAllTiles((x, y) => FOVTiles[x, y] = new TileFOVInfo(x, y));

            FOVRecurse = new FOVRecurse(BoardSize.Width, BoardSize.Height, playerComp.VisualRange);

            BoardSys.ForAllTiles(BoardSize, tilePos =>
                FOVRecurse.Point_Set(tilePos.X, tilePos.Y, !BoardSys.IsTile(boardComp.Board, tilePos) ? 1 : 0));
        }

        public void RevealAll() =>
            ForAllTiles((x, y) => FOVTiles[x, y].Reveal());

        public TileFOVInfo GetTileFOVInfo(int x, int y) => FOVTiles[x, y];

        public void SetPlayerPos(PlayerComp playerComp, IPositionComp playerPosComp)
        {
            Reset();
            FOVRecurse.SetPlayerPos(playerPosComp.TilePos.X, playerPosComp.TilePos.Y);
            ForAllTiles(tileFOVInfo =>
            {
                tileFOVInfo.Visible = FOVRecurse.VisiblePoints.Contains(tileFOVInfo.TilePos);
                tileFOVInfo.DistanceFactor =
                    Distance.Get(tileFOVInfo.Position, playerPosComp.Position)
                    / (BoardComp.TILE_SIZE * playerComp.VisualRange);
            });
        }

        public bool IsVisibleByPlayer(IPositionComp positionComp) =>
             FOVTiles[positionComp.TilePos.X, positionComp.TilePos.Y].VisibleByPlayer;

        public bool IsVisible(IPositionComp positionComp) =>
            IsVisible(positionComp.TilePos);

        private bool IsVisible(Point tilePos) =>
            FOVTiles[tilePos.X, tilePos.Y].Visible;

        private void Reset() =>
            ForAllTiles((x, y) => Reset(FOVTiles[x, y]));

        private static void Reset(TileFOVInfo tileFOVInfo)
        {
            tileFOVInfo.VisibleByPlayer = false;
            tileFOVInfo.DistanceFactor = 0;
        }

        public void ForAllTiles(Action<TileFOVInfo> action)
            => ForAllTiles((x, y) => action(FOVTiles[x, y]));

        private void ForAllTiles(Action<int, int> action)
        {
            for (var x = 0; x < BoardSize.Width; x++)
                for (var y = 0; y < BoardSize.Height; y++)
                    action(x, y);
        }
    }
}
