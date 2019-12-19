using Rogueskiv.Core.Components.Position;

namespace Rogueskiv.Core.Components.Walls
{
    abstract class VerticalWallComp : WallComp
    {
        protected VerticalWallComp(int x, int y, int height)
            : base(x, y, height) { }

        protected override float FixedPosition => X;
        protected override float VariablePosition => Y;

        protected override float GetFixedPosition(PositionComp position) => position.X;
        protected override float GetVariablePosition(PositionComp position) => position.Y;

        protected override void ReverseSpeed(MovementComp movement, float amortiguationFactor) =>
            movement.SpeedX *= amortiguationFactor;

        protected override void SetPosition(PositionComp position, float value) => position.X = value;
    }
}
