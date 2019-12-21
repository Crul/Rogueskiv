using Rogueskiv.Core.Components.Position;
using System.Collections.Generic;

namespace Rogueskiv.Core.Components.Walls
{
    abstract class HorizontalWallComp : WallComp
    {
        protected HorizontalWallComp(
            int x, int y, int width, WallFacingDirections facing, List<WallTile> tiles
        ) : base(x, y, width, facing, tiles) { }

        protected override float FixedPosition => Y;
        protected override float VariablePosition => X;

        protected override float GetFixedPosition(PositionComp position) => position.Y;
        protected override float GetVariablePosition(PositionComp position) => position.X;

        protected override void ReverseSpeed(MovementComp movement, float amortiguationFactor) =>
            movement.SpeedY *= amortiguationFactor;

        protected override void SetPosition(PositionComp position, float value) => position.Y = value;
    }
}
