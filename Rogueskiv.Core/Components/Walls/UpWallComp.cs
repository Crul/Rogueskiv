using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using System.Collections.Generic;
using System.Drawing;

namespace Rogueskiv.Core.Components.Walls
{
    class UpWallComp : HorizontalWallComp
    {
        public UpWallComp(Point tilePos, int height, List<WallTile> tiles)
            : base(tilePos, height, WallFacingDirections.UP, tiles)
            => BounceLimit = FixedPosition + BoardComp.TILE_SIZE;

        protected override float GetCollisionPosition(PositionComp positionComp, MovementComp movementComp)
            => GetFixedPosition(positionComp) + movementComp.Radius;

        protected override bool HasTraversed
            (CurrentPositionComp currentPositionComp, LastPositionComp lastPositionComp, MovementComp movementComp) =>
            GetCollisionPosition(lastPositionComp, movementComp) <= BounceLimit
            && GetCollisionPosition(currentPositionComp, movementComp) > BounceLimit;
    }
}
