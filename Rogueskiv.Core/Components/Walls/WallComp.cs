using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Seedwork.Crosscutting;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Rogueskiv.Core.Components.Walls
{
    public enum WallCorner
    {
        TOP_LEFT,
        TOP_RIGHT,
        BOTTOM_LEFT,
        BOTTOM_RIGHT,
    }

    public abstract class WallComp : TilePositionComp, IWallComp
    {
        public int Size { get; } // height for VerticalWalls, width for HorizontalWalls
        protected abstract float FixedPosition { get; }
        protected abstract float VariablePosition { get; }

        protected float BounceLimit; // set by children

        private const float PRECISE_BOUNCE_MARGIN_FACTOR = 1.05f;

        protected WallComp(Point tilePos, int size) : base(tilePos) =>
            Size = BoardComp.TILE_SIZE * size;

        public abstract List<Point> GetTiles();

        public bool CheckBounce(
            MovementComp movementComp,
            CurrentPositionComp currentPositionComp,
            LastPositionComp lastPositionComp
        ) =>
            movementComp.SimpleBounce
                ? CheckSimpleBounce(movementComp, currentPositionComp, lastPositionComp)
                : CheckPreciseBounce(movementComp, currentPositionComp, lastPositionComp);

        private bool CheckSimpleBounce(
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

            var currentFixedPos = GetFixedPosition(currentPositionComp.Position);
            var currentVarPos = GetVariablePosition(currentPositionComp.Position);

            var lastFixedPos = GetFixedPosition(lastPositionComp.Position);
            var lastVarPos = GetVariablePosition(lastPositionComp.Position);

            var deltaFixed = (currentFixedPos - lastFixedPos);
            var deltaVar = (currentVarPos - lastVarPos);

            var fromLastToWallFixedPos = (BounceLimit - GetFixedMargin(movementComp) - lastFixedPos);
            var variablePosCrossingWall = lastVarPos + deltaVar * (fromLastToWallFixedPos / deltaFixed);

            var minVarPos = VariablePosition;
            var maxVarPos = VariablePosition + Size;

            var isInFrontOrBehind = (
                (variablePosCrossingWall + movementComp.Radius) >= minVarPos
                && (variablePosCrossingWall - movementComp.Radius) <= maxVarPos
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
            var variablePosCrossingWall = deltaFixed != 0
                ? lastVarPos + deltaVar * (fromLastToWallFixedPos / deltaFixed)
                : 0; // TODO division by 0?

            var minVarPos = VariablePosition;
            var maxVarPos = VariablePosition + Size;

            var isInFrontOrBehind = (
                variablePosCrossingWall > minVarPos
                && variablePosCrossingWall < maxVarPos
            );
            if (isInFrontOrBehind)
            {
                var hasTraversed = HasTraversed(currentPositionComp, lastPositionComp, movementComp);
                if (!hasTraversed)
                    return false;

                ReverseSpeed(movementComp, -movementComp.BounceAmortiguationFactor);
                SimpleBounce(movementComp, currentPositionComp);
                return true;
            }

            PointF? advancedBouncePosition = null;
            bool? startOrEnd = null;
            var isInStartCorner = (Math.Abs(minVarPos - variablePosCrossingWall) < movementComp.Radius);
            if (isInStartCorner)
            {
                startOrEnd = true;
                advancedBouncePosition = GetStartPosition(movementComp);
            }
            else
            {
                var isInEndCorner = (Math.Abs(maxVarPos - variablePosCrossingWall) < movementComp.Radius);
                if (isInEndCorner)
                {
                    startOrEnd = false;
                    advancedBouncePosition = GetEndPosition(movementComp);
                }
            }

            if (advancedBouncePosition.HasValue)
                // do not return true because no more checks needed (I hope)
                PreciseBounce(
                    movementComp,
                    currentPositionComp,
                    lastPositionComp,
                    advancedBouncePosition.Value,
                    startOrEnd.Value
                );

            return false;
        }

        private void PreciseBounce(
            MovementComp movementComp,
            CurrentPositionComp currentPositionComp,
            LastPositionComp lastPositionComp,
            PointF bouncePosition,
            bool startOrEnd
        )
        {
            var goingAwayFromCorner = false;
            switch (GetConvexCorner(startOrEnd))
            {
                case WallCorner.TOP_LEFT:
                    goingAwayFromCorner = (lastPositionComp.Position.X > currentPositionComp.Position.X)
                        && (lastPositionComp.Position.Y > currentPositionComp.Position.Y);
                    break;
                case WallCorner.TOP_RIGHT:
                    goingAwayFromCorner = (lastPositionComp.Position.X < currentPositionComp.Position.X)
                        && (lastPositionComp.Position.Y > currentPositionComp.Position.Y);
                    break;
                case WallCorner.BOTTOM_LEFT:
                    goingAwayFromCorner = (lastPositionComp.Position.X > currentPositionComp.Position.X)
                        && (lastPositionComp.Position.Y < currentPositionComp.Position.Y);
                    break;
                case WallCorner.BOTTOM_RIGHT:
                    goingAwayFromCorner = (lastPositionComp.Position.X < currentPositionComp.Position.X)
                        && (lastPositionComp.Position.Y < currentPositionComp.Position.Y);
                    break;
            }

            var distance = Distance.Get(lastPositionComp.Position, bouncePosition);
            if (goingAwayFromCorner || distance > movementComp.Radius)
                return;

            // TODO get proper bouncing angle
            var angle = Math.Atan2(
                lastPositionComp.Position.Y - bouncePosition.Y,
                lastPositionComp.Position.X - bouncePosition.X
            );
            var cosAngle = (float)Math.Cos(angle);
            var sinAngle = (float)Math.Sin(angle);
            currentPositionComp.SetPosition(
                bouncePosition.X + (cosAngle * movementComp.Radius * PRECISE_BOUNCE_MARGIN_FACTOR),
                bouncePosition.Y + (sinAngle * movementComp.Radius * PRECISE_BOUNCE_MARGIN_FACTOR)
            );

            var speed = movementComp.BounceAmortiguationFactor * Distance.Get(movementComp.Speed);
            movementComp.Speed = new PointF(
                speed * cosAngle,
                speed * sinAngle
            );
        }

        protected abstract PointF GetStartPosition(MovementComp movementComp);
        protected abstract PointF GetEndPosition(MovementComp movementComp);
        protected abstract float GetFixedMargin(MovementComp movementComp);
        protected abstract float GetFixedPosition(PointF position);
        protected abstract float GetVariablePosition(PointF position);
        protected abstract void SetFixedPosition(PositionComp positionComp, float value);
        protected abstract void SetVariablePosition(PositionComp positionComp, float value);
        protected abstract WallCorner GetConvexCorner(bool startOrEndCorner);

        protected abstract bool HasTraversed(
            CurrentPositionComp currentPositionComp, LastPositionComp lastPositionComp, MovementComp movementComp
        );

        protected abstract void ReverseSpeed(MovementComp movementComp, float amortiguationFactor);
    }
}
