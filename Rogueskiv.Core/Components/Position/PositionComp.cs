using Rogueskiv.Core.Components.Board;
using Seedwork.Crosscutting;
using System.Drawing;

namespace Rogueskiv.Core.Components.Position
{
    public abstract class PositionComp : IPositionComp
    {
        private PointF position;
        public PointF Position
        {
            get => position;
            set
            {
                position = value;
                TilePos = position.Divide(BoardComp.TILE_SIZE).ToPoint();
            }
        }

        public Point TilePos { get; private set; }

        protected PositionComp(Point tilePos)
            : this(tilePos.Multiply(BoardComp.TILE_SIZE).Add(BoardComp.TILE_SIZE / 2)) { }

        protected PositionComp(PointF position) => Position = position;

        public void SetPosition(float? x = null, float? y = null) =>
             Position = new PointF(x ?? Position.X, y ?? Position.Y);
    }
}
