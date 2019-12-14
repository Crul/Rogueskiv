using Rogueskiv.Core.Components.Position;

namespace Rogueskiv.Core.Components.Walls
{
    abstract class HorizontalWallComp : WallComp
    {
        public HorizontalWallComp(int x, int y, int width)
            : base(x, y, width) { }

        protected override float FixedPosition => Y;
        protected override float VariablePosition => X;

        protected override float GetFixedPosition(PositionComp position) => position.Y;
        protected override float GetVariablePosition(PositionComp position) => position.X;

        protected override void ReverseSpeed(MovementComp movement, float amortiguationFactor) =>
            movement.SpeedY *= amortiguationFactor;

        protected override void SetPosition(PositionComp position, float value) => position.Y = value;
    }
}
