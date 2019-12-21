using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using System.Collections.Generic;

namespace Rogueskiv.Core.Components.Walls
{
    class LeftWallComp : VerticalWallComp
    {
        public LeftWallComp(int x, int y, int height, List<WallTile> tiles)
            : base(x, y, height, WallFacingDirections.LEFT, tiles)
            => BounceLimit = FixedPosition + BoardComp.TILE_SIZE - (ENTITY_SIZE / 2);

        protected override bool HasTraversed
            (PositionComp position, PositionComp oldPosition) =>
            (oldPosition.X <= BounceLimit && position.X > BounceLimit);
    }
}
