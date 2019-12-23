using Rogueskiv.Core.Components.Position;
using System.Collections.Generic;
using System.Drawing;

namespace Rogueskiv.Core.Components.Walls
{
    class RightWallComp : VerticalWallComp
    {
        public RightWallComp(Point tilePos, int height, List<WallTile> tiles)
            : base(tilePos, height, WallFacingDirections.RIGHT, tiles)
            => BounceLimit = FixedPosition + (ENTITY_SIZE / 2);

        protected override bool HasTraversed
            (CurrentPositionComp currentPositionComp, LastPositionComp lastPositionComp) =>
            (lastPositionComp.Position.X >= BounceLimit && currentPositionComp.Position.X < BounceLimit);
    }
}
