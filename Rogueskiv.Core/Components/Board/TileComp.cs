using Rogueskiv.Core.Components.Position;
using Seedwork.Crosscutting;
using System;
using System.Drawing;

namespace Rogueskiv.Core.Components.Board
{
    [Flags]
    public enum TileWallFlags : uint
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8,
        CornerUpRight = 16,
        CornerUpLeft = 32,
        CornerDownRight = 64,
        CornerDownLeft = 128,
    };

    public class TileComp : PositionComp
    {
        public bool HasBeenSeen { get; set; }
        public bool VisibleByPlayer { get; private set; }
        public TileWallFlags WallFlags { get; private set; }
        public bool IsWall { get => WallFlags != TileWallFlags.None; }

        public override bool Visible
        {
            get => HasBeenSeen;
            set
            {
                VisibleByPlayer = value;
                HasBeenSeen = HasBeenSeen || value;
            }
        }

        public TileComp(Point tilePos)
            : base(tilePos.Multiply(BoardComp.TILE_SIZE).Add(BoardComp.TILE_SIZE / 2)) { }

        public void AddWall(TileWallFlags wallFlag) => WallFlags |= wallFlag;
    }
}
