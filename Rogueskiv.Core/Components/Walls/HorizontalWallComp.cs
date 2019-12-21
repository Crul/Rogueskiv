using Rogueskiv.Core.Components.Position;
using System.Collections.Generic;
using System.Drawing;

namespace Rogueskiv.Core.Components.Walls
{
    abstract class HorizontalWallComp : WallComp
    {
        protected HorizontalWallComp(
            Point tilePos, int width, WallFacingDirections facing, List<WallTile> tiles
        ) : base(tilePos, width, facing, tiles) { }

        protected override float FixedPosition => Position.Y;
        protected override float VariablePosition => Position.X;

        protected override float GetFixedPosition(PositionComp positionComp) =>
            positionComp.Position.Y;
        protected override float GetVariablePosition(PositionComp positionComp) =>
            positionComp.Position.X;

        protected override void ReverseSpeed(MovementComp movement, float amortiguationFactor) =>
            movement.MultiplySpeed(factorY: amortiguationFactor);

        protected override void SetPosition(PositionComp positionComp, float value) =>
            positionComp.SetPosition(y: value);
    }
}
