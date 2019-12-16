using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;

namespace Rogueskiv.Core.Components.Walls
{
    class LeftWallComp : VerticalWallComp
    {
        public LeftWallComp(int x, int y, int height)
            : base(x, y, height)
            => BounceLimit = FixedPosition + BoardComp.TILE_SIZE - (ENTITY_SIZE / 2);

        protected override bool HasTraversed
            (PositionComp position, PositionComp oldPosition) =>
            (oldPosition.X <= BounceLimit && position.X > BounceLimit);
    }
}
