using Rogueskiv.Core.Components.Position;
using System.Drawing;

namespace Rogueskiv.Core.Components.Walls
{
    abstract class VerticalWallComp : WallComp
    {
        protected VerticalWallComp(Point tilePos, int height)
            : base(tilePos, height) { }

        protected override float FixedPosition => Position.X;
        protected override float VariablePosition => Position.Y;

        protected override float GetFixedPosition(PositionComp positionComp) =>
            positionComp.Position.X;
        protected override float GetVariablePosition(PositionComp positionComp) =>
            positionComp.Position.Y;

        protected override void ReverseSpeed(MovementComp movementComp, float amortiguationFactor) =>
            movementComp.MultiplySpeed(factorX: amortiguationFactor);

        protected override void SetFixedPosition(PositionComp positionComp, float value) =>
            positionComp.SetPosition(x: value);
    }
}
