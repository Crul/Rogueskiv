using System.Drawing;

namespace Rogueskiv.Core.Components.Position
{
    public abstract class PositionComp : IPositionComp
    {
        public PointF Position { get; set; } // TODO private set
        public virtual bool Visible { get; set; } = true;

        protected PositionComp(PointF position) =>
            Position = position;
    }
}
