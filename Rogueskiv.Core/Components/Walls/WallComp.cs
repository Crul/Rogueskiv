using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using System.Drawing;

namespace Rogueskiv.Core.Components.Walls
{
    public abstract class WallComp : PositionComp, IWallComp
    {
        protected const int WALL_THICKNESS = 20;

        public int Size { get; } // height for VerticalWalls, width for HorizontalWalls
        public PositionComp PositionComp => this;

        protected abstract float FixedPosition { get; }
        protected abstract float VariablePosition { get; }


        protected float BounceLimit; // set by children

        protected WallComp(Point tilePos, int size) : base(tilePos) =>
            Size = BoardComp.TILE_SIZE * size;

        public bool CheckBounce(
            MovementComp movementComp,
            CurrentPositionComp currentPositionComp,
            LastPositionComp lastPositionComp
        )
        {
            var hasTraversed = HasTraversed(currentPositionComp, lastPositionComp, movementComp);
            if (!hasTraversed)
                return false;

            // for HorizontalWalls: check if position.X when crossing the limit is between wall.minX and wall.maxX
            // for VerticalWalls  : check if position.Y when crossing the limit is between wall.minY and wall.maxY

            var currentFixedPos = GetFixedPosition(currentPositionComp);
            var currentVarPos = GetVariablePosition(currentPositionComp);

            var lastFixedPos = GetFixedPosition(lastPositionComp);
            var lastVarPos = GetVariablePosition(lastPositionComp);

            var deltaFixed = (currentFixedPos - lastFixedPos);
            var deltaVar = (currentVarPos - lastVarPos);

            var fromLastToWallFixedPos = (BounceLimit - GetFixedMargin(movementComp) - lastFixedPos);
            var variablePosCrossingWall = lastVarPos + deltaVar * (fromLastToWallFixedPos / deltaFixed);

            var minVarPos = VariablePosition - WALL_THICKNESS;
            var maxVarPos = VariablePosition + WALL_THICKNESS + Size;

            var isInFrontOrBehind = (
                (variablePosCrossingWall + movementComp.Radius) > minVarPos
                && (variablePosCrossingWall - movementComp.Radius) < maxVarPos
            );
            if (!isInFrontOrBehind)
                return false;

            // TODO bounces in corners (w/ angle)
            ReverseSpeed(movementComp, -movementComp.BounceAmortiguationFactor);
            var newFixedPosition =
                2 * (BounceLimit - GetFixedMargin(movementComp)) - currentFixedPos;

            SetFixedPosition(currentPositionComp, newFixedPosition);

            return true;
        }

        protected abstract float GetFixedMargin(MovementComp movementComp);

        protected abstract float GetFixedPosition(PositionComp positionComp);
        protected abstract void SetFixedPosition(PositionComp positionComp, float value);
        protected abstract float GetVariablePosition(PositionComp positionComp);

        protected abstract bool HasTraversed(
            CurrentPositionComp currentPositionComp, LastPositionComp lastPositionComp, MovementComp movementComp
        );

        protected abstract void ReverseSpeed(MovementComp movementComp, float amortiguationFactor);
    }
}
