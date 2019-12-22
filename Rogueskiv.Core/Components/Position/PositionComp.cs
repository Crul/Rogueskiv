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
                TilePos = position.Divide(BoardComp.TILE_SIZE);
            }
        }

        public Point TilePos { get; private set; }

        public virtual bool Visible { get; set; } = true;

        protected PositionComp(PointF position) =>
            Position = position;

        protected PositionComp(Point tilePos) =>
            Position = tilePos.Multiply(BoardComp.TILE_SIZE);

        public void SetPosition(float? x = null, float? y = null) =>
             Position = new PointF(x ?? Position.X, y ?? Position.Y);
    }
}
