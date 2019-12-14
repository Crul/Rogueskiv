using Rogueskiv.Core.Components.Position;

namespace Rogueskiv.Core.Components.Walls
{
    class DownWallComp : HorizontalWallComp
    {
        public DownWallComp(int x, int y, int height)
            : base(x, y, height)
            => BounceLimit = FixedPosition + (ENTITY_SIZE / 2);

        protected override bool HasTraversed
            (PositionComp position, PositionComp oldPosition) =>
            (oldPosition.Y >= BounceLimit && position.Y < BounceLimit);
    }
}
