using Rogueskiv.Core.Components.Position;

namespace Rogueskiv.Core.Components.Walls
{
    abstract class WallComp : PositionComp, IWallComp
    {
        // TODO ¿move to system?
        protected const int ENTITY_SIZE = 16; // TODO proper entity size
        private const int TILE_SIZE = 30;     // TODO proper tile size

        public int Size { get; } // height for VerticalWalls, width for HorizontalWalls
        public PositionComp Position => this;

        protected abstract float FixedPosition { get; }
        protected abstract float VariablePosition { get; }


        protected float BounceLimit; // set by children

        protected WallComp(int x, int y, int size)
        {
            X = TILE_SIZE * x;
            Y = TILE_SIZE * y;
            Size = TILE_SIZE * size;
        }

        public bool CheckBounce(
            MovementComp movement,
            PositionComp position,
            PositionComp oldPosition
        )
        {
            var minPosLength = VariablePosition;
            var maxPosLength = VariablePosition + Size;
            var entityPos = GetVariablePosition(position);
            // for HorizontalWalls: check if entity.position.x is between wall.minX and wall.maxX
            // for VerticalWalls  : check if entity.position.y is between wall.minY and wall.maxY
            var isInFrontOrBehind = (
                (entityPos + (ENTITY_SIZE / 2)) > minPosLength
                && (entityPos - (ENTITY_SIZE / 2)) < maxPosLength
            );

            // TODO bounces in corners (w/ angle)
            var bounce = isInFrontOrBehind && HasTraversed(position, oldPosition);
            if (bounce)
            {
                ReverseSpeed(movement, -movement.BounceAmortiguationFactor);
                SetPosition(position, (2 * BounceLimit) - GetFixedPosition(position));
            }

            return bounce;
        }

        protected abstract float GetFixedPosition(PositionComp position);
        protected abstract float GetVariablePosition(PositionComp position);

        protected abstract bool HasTraversed(PositionComp position, PositionComp oldPosition);

        protected abstract void ReverseSpeed(MovementComp movement, float amortiguationFactor);

        protected abstract void SetPosition(PositionComp position, float value);
    }
}
