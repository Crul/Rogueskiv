using Rogueskiv.Core.Components.Position;
using Seedwork.Crosscutting;
using System.Drawing;

namespace Rogueskiv.Core.Components.Walls
{
    abstract class HorizontalWallComp : WallComp
    {
        protected HorizontalWallComp(Point tilePos, int width)
            : base(tilePos, width) { }

        protected override float FixedPosition => Position.Y;
        protected override float VariablePosition => Position.X;

        protected override PointF GetStartPosition(MovementComp movementComp) =>
            new PointF(x: Position.X, y: BounceLimit);

        protected override PointF GetEndPosition(MovementComp movement) =>
            GetStartPosition(movement).Add(x: Size + 2);

        protected override float GetFixedPosition(PointF position) => position.Y;
        protected override float GetVariablePosition(PointF position) => position.X;

        protected override void SetFixedPosition(PositionComp positionComp, float value) =>
            positionComp.SetPosition(y: value);
        protected override void SetVariablePosition(PositionComp positionComp, float value) =>
            positionComp.SetPosition(x: value);

        protected override void ReverseSpeed(MovementComp movementComp, float amortiguationFactor) =>
            movementComp.MultiplySpeed(factorY: amortiguationFactor);
    }
}
