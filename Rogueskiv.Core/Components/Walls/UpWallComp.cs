using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using System.Collections.Generic;

namespace Rogueskiv.Core.Components.Walls
{
    class UpWallComp : HorizontalWallComp
    {
        public UpWallComp(int x, int y, int height, List<WallTile> tiles)
            : base(x, y, height, WallFacingDirections.UP, tiles)
            => BounceLimit = FixedPosition + BoardComp.TILE_SIZE - (ENTITY_SIZE / 2);

        protected override bool HasTraversed
            (PositionComp position, PositionComp oldPosition) =>
            (oldPosition.Y <= BounceLimit && position.Y > BounceLimit);
    }
}
