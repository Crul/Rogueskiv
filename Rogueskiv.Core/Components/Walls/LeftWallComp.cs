using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using System.Drawing;

namespace Rogueskiv.Core.Components.Walls
{
    class LeftWallComp : VerticalWallComp
    {
        public LeftWallComp(Point tilePos, int height) : base(tilePos, height)
            => BounceLimit = FixedPosition + BoardComp.TILE_SIZE;

        protected override float GetFixedMargin(MovementComp movementComp)
            => movementComp.Radius;

        protected override bool HasTraversed
            (CurrentPositionComp currentPositionComp, LastPositionComp lastPositionComp, MovementComp movementComp) =>
            GetFixedPosition(lastPositionComp.Position) + GetFixedMargin(movementComp) <= BounceLimit
            && GetFixedPosition(currentPositionComp.Position) + GetFixedMargin(movementComp) > BounceLimit;
    }
}
