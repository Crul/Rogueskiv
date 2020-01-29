using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Seedwork.Crosscutting;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Rogueskiv.Core.Components.Walls
{
    abstract class VerticalWallComp : WallComp
    {
        protected override float FixedPosition => Position.X;
        protected override float VariablePosition => Position.Y;

        protected VerticalWallComp(Point tilePos, int height)
            : base(tilePos, height) { }

        public override List<Point> GetTiles() =>
            Enumerable.Range(0, Size / BoardComp.TILE_SIZE).Select(i => TilePos.Add(y: i)).ToList();

        protected override PointF GetStartPosition(MovementComp movementComp) =>
            new PointF(x: BounceLimit, y: Position.Y);

        protected override PointF GetEndPosition(MovementComp movement) =>
            GetStartPosition(movement).Add(y: Size);

        protected override float GetFixedPosition(PointF position) => position.X;
        protected override float GetVariablePosition(PointF position) => position.Y;

        protected override void ReverseSpeed(MovementComp movementComp, float momentumConservationFactor) =>
            movementComp.MultiplySpeed(factorX: momentumConservationFactor);

        protected override void SetFixedPosition(PositionComp positionComp, float value) =>
            positionComp.SetPosition(x: value);
        protected override void SetVariablePosition(PositionComp positionComp, float value) =>
            positionComp.SetPosition(y: value);
    }
}
