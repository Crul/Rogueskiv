﻿using Rogueskiv.Core.Components.Board;
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
        private const int VISUAL_RANGE = 10; // TODO proper visual range

        private FOVRecurse FOVRecurse;
        private Size BoardSize;

        public TileFOVInfo[,] FOVTiles { get; private set; }

        public void Init(BoardComp boardComp)
        {
            BoardSize = boardComp.BoardSize;

            FOVTiles = new TileFOVInfo[BoardSize.Width, BoardSize.Height];
            ForAllTiles((x, y) => FOVTiles[x, y] = new TileFOVInfo());

            FOVRecurse = new FOVRecurse(BoardSize.Width, BoardSize.Height, VISUAL_RANGE);

            BoardSys.ForAllTiles(BoardSize, tilePos =>
                FOVRecurse.Point_Set(tilePos.X, tilePos.Y, !BoardSys.IsTile(boardComp.Board, tilePos) ? 1 : 0));
        }

        public void SetPlayerPos(PositionComp positionComp) =>
            FOVRecurse.SetPlayerPos(positionComp.TilePos.X, positionComp.TilePos.Y);

        public bool IsVisible(PositionComp positionComp) =>
            FOVRecurse.VisiblePoints.Any(vp => vp == positionComp.TilePos);

        public void Reset() =>
            ForAllTiles((x, y) => Reset(FOVTiles[x, y]));

        private void Reset(TileFOVInfo tileFOVInfo)
        {
            tileFOVInfo.Hidden = false;
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