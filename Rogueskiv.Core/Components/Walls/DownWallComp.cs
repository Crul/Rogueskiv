using Rogueskiv.Core.Components.Position;
using System.Collections.Generic;
using System.Drawing;

namespace Rogueskiv.Core.Components.Walls
{
    class DownWallComp : HorizontalWallComp
    {
        public DownWallComp(Point tilePos, int height, List<WallTile> tiles)
            : base(tilePos, height, WallFacingDirections.DOWN, tiles)
            => BounceLimit = FixedPosition + (ENTITY_SIZE / 2);

        protected override bool HasTraversed
            (PositionComp positionComp, PositionComp oldPositionComp) =>
            (oldPositionComp.Position.Y >= BounceLimit && positionComp.Position.Y < BounceLimit);
    }
}
