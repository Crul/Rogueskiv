using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using System;
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

            return movementComp.SimpleBounce
                ? CheckSimpleBounce(movementComp, currentPositionComp, lastPositionComp)
                : CheckPreciseBounce(movementComp, currentPositionComp, lastPositionComp);
        }

        private bool CheckSimpleBounce(
            MovementComp movementComp,
            CurrentPositionComp currentPositionComp,
            LastPositionComp lastPositionComp
        )
        {
            // for HorizontalWalls: check if position.X when crossing the limit is between wall.minX and wall.maxX
            // for VerticalWalls  : check if position.Y when crossing the limit is between wall.minY and wall.maxY

            var currentFixedPos = GetFixedPosition(currentPositionComp.Position);
            var currentVarPos = GetVariablePosition(currentPositionComp.Position);

            var lastFixedPos = GetFixedPosition(lastPositionComp.Position);
            var lastVarPos = GetVariablePosition(lastPositionComp.Position);

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

            ReverseSpeed(movementComp, -movementComp.BounceAmortiguationFactor);
            SimpleBounce(movementComp, currentPositionComp);

            return true;
        }

        private void SimpleBounce(MovementComp movementComp, CurrentPositionComp currentPositionComp)
        {
            var currentFixedPos = GetFixedPosition(currentPositionComp.Position);
            var newFixedPosition =
                2 * (BounceLimit - GetFixedMargin(movementComp)) - currentFixedPos;

            SetFixedPosition(currentPositionComp, newFixedPosition);
        }

        private bool CheckPreciseBounce(
            MovementComp movementComp,
            CurrentPositionComp currentPositionComp,
            LastPositionComp lastPositionComp
        )
        {
            var currentFixedPos = GetFixedPosition(currentPositionComp.Position);
            var currentVarPos = GetVariablePosition(currentPositionComp.Position);

            var lastFixedPos = GetFixedPosition(lastPositionComp.Position);
            var lastVarPos = GetVariablePosition(lastPositionComp.Position);

            var deltaFixed = (currentFixedPos - lastFixedPos);
            var deltaVar = (currentVarPos - lastVarPos);

            var fixedMargin = GetFixedMargin(movementComp);
            var fromLastToWallFixedPos = BounceLimit - fixedMargin - lastFixedPos;
            var variablePosCrossingWall = lastVarPos + deltaVar * (fromLastToWallFixedPos / deltaFixed);

            var minVarPos = VariablePosition - WALL_THICKNESS;
            var maxVarPos = VariablePosition + WALL_THICKNESS + Size;

            var isInFrontOrBehind = (
                variablePosCrossingWall > minVarPos
                && variablePosCrossingWall < maxVarPos
            );
            if (isInFrontOrBehind)
            {
                ReverseSpeed(movementComp, -movementComp.BounceAmortiguationFactor);
                SimpleBounce(movementComp, currentPositionComp);
                return true;
            }

            var variableMarginSign = 1;
            PointF? advancedBouncPosition = null;
            var isInStartCorner = (Math.Abs(minVarPos - variablePosCrossingWall) < movementComp.Radius);
            if (isInStartCorner)
                advancedBouncPosition = GetStartPosition(movementComp);
            else
            {
                variableMarginSign = -1;
                var isInEndCorner = (Math.Abs(maxVarPos - variablePosCrossingWall) < movementComp.Radius);
                if (isInEndCorner)
                    advancedBouncPosition = GetEndPosition(movementComp);
            }

            if (advancedBouncPosition.HasValue)
                // do not return true because no more checks needed (I hope)
                AdvancedBounce(
                    movementComp, currentPositionComp, lastPositionComp, advancedBouncPosition.Value,
                    variableMarginSign
                );

            return false;
        }

        private void AdvancedBounce(
            MovementComp movementComp,
            CurrentPositionComp currentPositionComp,
            LastPositionComp lastPositionComp,
            PointF bouncePosition,
            int variableMarginSign = 1

        )
        {
            var angle = Math.Atan2(
                lastPositionComp.Position.Y - bouncePosition.Y,
                lastPositionComp.Position.X - bouncePosition.X
            );
            var cosAngle = (float)Math.Cos(angle);
            var sinAngle = (float)Math.Sin(angle);
            SimpleBounce(movementComp, currentPositionComp);
            SetVariablePosition(currentPositionComp,
                GetVariablePosition(bouncePosition) - (variableMarginSign * movementComp.Radius)
            );

            var speed =
                movementComp.BounceAmortiguationFactor
                * (float)Math.Sqrt(
                    movementComp.Speed.X * movementComp.Speed.X
                    + movementComp.Speed.Y * movementComp.Speed.Y
                );
            var newSpeedX = speed * cosAngle;
            var newSpeedY = speed * sinAngle;
            movementComp.Speed = new PointF(newSpeedX, newSpeedY);
        }

        protected abstract PointF GetStartPosition(MovementComp movementComp);
        protected abstract PointF GetEndPosition(MovementComp movementComp);
        protected abstract float GetFixedMargin(MovementComp movementComp);
        protected abstract float GetFixedPosition(PointF position);
        protected abstract float GetVariablePosition(PointF position);
        protected abstract void SetFixedPosition(PositionComp positionComp, float value);
        protected abstract void SetVariablePosition(PositionComp positionComp, float value);

        protected abstract bool HasTraversed(
            CurrentPositionComp currentPositionComp, LastPositionComp lastPositionComp, MovementComp movementComp
        );

        protected abstract void ReverseSpeed(MovementComp movementComp, float amortiguationFactor);
    }
}
