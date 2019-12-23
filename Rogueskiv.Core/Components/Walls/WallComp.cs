using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using System.Collections.Generic;
using System.Drawing;

namespace Rogueskiv.Core.Components.Walls
{
    public abstract class WallComp : PositionComp, IWallComp
    {
        // TODO ¿move to system?
        protected const int ENTITY_SIZE = 16; // TODO proper entity size

        public int Size { get; } // height for VerticalWalls, width for HorizontalWalls
        public PositionComp PositionComp => this;
        public WallFacingDirections Facing { get; }
        public List<WallTile> Tiles { get; }

        protected abstract float FixedPosition { get; }
        protected abstract float VariablePosition { get; }


        protected float BounceLimit; // set by children

        protected WallComp(
            Point tilePos, int size, WallFacingDirections facing, List<WallTile> tiles
        ) : base(tilePos)
        {
            Size = BoardComp.TILE_SIZE * size;
            Facing = facing;
            Tiles = tiles;
        }

        public bool CheckBounce(
            MovementComp movementComp,
            CurrentPositionComp currentPositionComp,
            LastPositionComp lastPositionComp
        )
        {
            var minPosLength = VariablePosition;
            var maxPosLength = VariablePosition + Size;
            var entityPos = GetVariablePosition(currentPositionComp);
            // for HorizontalWalls: check if entity.position.x is between wall.minX and wall.maxX
            // for VerticalWalls  : check if entity.position.y is between wall.minY and wall.maxY
            var isInFrontOrBehind = (
                (entityPos + (ENTITY_SIZE / 2)) > minPosLength
                && (entityPos - (ENTITY_SIZE / 2)) < maxPosLength
            );

            // TODO bounces in corners (w/ angle)
            var bounce = isInFrontOrBehind && HasTraversed(currentPositionComp, lastPositionComp);
            if (bounce)
            {
                ReverseSpeed(movementComp, -movementComp.BounceAmortiguationFactor);
                SetPosition(currentPositionComp, (2 * BounceLimit) - GetFixedPosition(currentPositionComp));
            }

            return bounce;
        }

        protected abstract float GetFixedPosition(PositionComp positionComp);
        protected abstract float GetVariablePosition(PositionComp positionComp);

        protected abstract bool HasTraversed(
            CurrentPositionComp currentPositionComp, LastPositionComp lastPositionComp
        );

        protected abstract void ReverseSpeed(MovementComp movementComp, float amortiguationFactor);

        protected abstract void SetPosition(PositionComp positionComp, float value);
    }
}
