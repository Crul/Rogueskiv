using Rogueskiv.Core.Components.Position;
using System.Collections.Generic;

namespace Rogueskiv.Core.Components.Walls
{
    class DownWallComp : HorizontalWallComp
    {
        public DownWallComp(int x, int y, int height, List<WallTile> tiles)
            : base(x, y, height, WallFacingDirections.DOWN, tiles)
            => BounceLimit = FixedPosition + (ENTITY_SIZE / 2);

        protected override bool HasTraversed
            (PositionComp position, PositionComp oldPosition) =>
            (oldPosition.Y >= BounceLimit && position.Y < BounceLimit);
    }
}
