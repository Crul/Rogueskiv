using Rogueskiv.Core.Components.Position;
using System.Collections.Generic;
using System.Drawing;

namespace Rogueskiv.Core.Components.Walls
{
    abstract class VerticalWallComp : WallComp
    {
        protected VerticalWallComp(
            Point tilePos, int height, WallFacingDirections facing, List<WallTile> tiles
        ) : base(tilePos, height, facing, tiles) { }

        protected override float FixedPosition => Position.X;
        protected override float VariablePosition => Position.Y;

        protected override float GetFixedPosition(PositionComp positionComp) =>
            positionComp.Position.X;
        protected override float GetVariablePosition(PositionComp positionComp) =>
            positionComp.Position.Y;

        protected override void ReverseSpeed(MovementComp movement, float amortiguationFactor) =>
            movement.Speed = new PointF(movement.Speed.X * amortiguationFactor, movement.Speed.Y);

        protected override void SetPosition(PositionComp positionComp, float value) =>
            positionComp.Position = new PointF(value, positionComp.Position.Y);
    }
}
