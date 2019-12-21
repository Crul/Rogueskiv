using Rogueskiv.Core.Components.Position;
using System.Collections.Generic;

namespace Rogueskiv.Core.Components.Walls
{
    class RightWallComp : VerticalWallComp
    {
        public RightWallComp(int x, int y, int height, List<WallTile> tiles)
            : base(x, y, height, WallFacingDirections.RIGHT, tiles)
            => BounceLimit = FixedPosition + (ENTITY_SIZE / 2);

        protected override bool HasTraversed
            (PositionComp position, PositionComp oldPosition) =>
            (oldPosition.X >= BounceLimit && position.X < BounceLimit);
    }
}
