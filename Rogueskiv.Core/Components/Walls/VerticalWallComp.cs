using Rogueskiv.Core.Components.Position;
using Seedwork.Crosscutting;
using System.Drawing;

namespace Rogueskiv.Core.Components.Walls
{
    abstract class VerticalWallComp : WallComp
    {
        protected VerticalWallComp(Point tilePos, int height)
            : base(tilePos, height) { }

        protected override float FixedPosition => Position.X;
        protected override float VariablePosition => Position.Y;

        protected override PointF GetStartPosition(MovementComp movementComp) =>
            new PointF(x: BounceLimit, y: Position.Y - WALL_THICKNESS);

        protected override PointF GetEndPosition(MovementComp movement) =>
            GetStartPosition(movement).Add(y: Size + 2 * WALL_THICKNESS);

        protected override float GetFixedPosition(PointF position) => position.X;
        protected override float GetVariablePosition(PointF position) => position.Y;

        protected override void ReverseSpeed(MovementComp movementComp, float amortiguationFactor) =>
            movementComp.MultiplySpeed(factorX: amortiguationFactor);

        protected override void SetFixedPosition(PositionComp positionComp, float value) =>
            positionComp.SetPosition(x: value);
        protected override void SetVariablePosition(PositionComp positionComp, float value) =>
            positionComp.SetPosition(y: value);
    }
}
