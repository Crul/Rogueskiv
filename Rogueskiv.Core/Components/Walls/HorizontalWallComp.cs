using Rogueskiv.Core.Components.Position;
using System.Drawing;

namespace Rogueskiv.Core.Components.Walls
{
    abstract class HorizontalWallComp : WallComp
    {
        protected HorizontalWallComp(Point tilePos, int width)
            : base(tilePos, width) { }

        protected override float FixedPosition => Position.Y;
        protected override float VariablePosition => Position.X;

        protected override float GetFixedPosition(PositionComp positionComp) =>
            positionComp.Position.Y;
        protected override float GetVariablePosition(PositionComp positionComp) =>
            positionComp.Position.X;

        protected override void ReverseSpeed(MovementComp movementComp, float amortiguationFactor) =>
            movementComp.MultiplySpeed(factorY: amortiguationFactor);

        protected override void SetFixedPosition(PositionComp positionComp, float value) =>
            positionComp.SetPosition(y: value);
    }
}
