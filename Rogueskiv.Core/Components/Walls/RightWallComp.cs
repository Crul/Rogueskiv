﻿using Rogueskiv.Core.Components.Position;
using System.Drawing;

namespace Rogueskiv.Core.Components.Walls
{
    class RightWallComp : VerticalWallComp
    {
        public RightWallComp(Point tilePos, int height) : base(tilePos, height)
            => BounceLimit = FixedPosition + WALL_THICKNESS;

        protected override float GetFixedMargin(MovementComp movementComp)
            => -movementComp.Radius;

        protected override bool HasTraversed
            (CurrentPositionComp currentPositionComp, LastPositionComp lastPositionComp, MovementComp movementComp) =>
            GetFixedPosition(lastPositionComp) + GetFixedMargin(movementComp) >= BounceLimit
            && GetFixedPosition(currentPositionComp) + GetFixedMargin(movementComp) < BounceLimit;
    }
}
