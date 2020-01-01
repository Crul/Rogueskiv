using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Seedwork.Crosscutting;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Rogueskiv.Core.Components.Walls
{
    abstract class HorizontalWallComp : WallComp
    {
        protected override float FixedPosition => Position.Y;
        protected override float VariablePosition => Position.X;

        protected HorizontalWallComp(Point tilePos, int width)
            : base(tilePos, width) { }

        public override List<Point> GetTiles() =>
            Enumerable.Range(0, Size / BoardComp.TILE_SIZE).Select(i => TilePos.Add(x: i)).ToList();

        protected override PointF GetStartPosition(MovementComp movementComp) =>
            new PointF(x: Position.X, y: BounceLimit);

        protected override PointF GetEndPosition(MovementComp movement) =>
            GetStartPosition(movement).Add(x: Size);

        protected override float GetFixedPosition(PointF position) => position.Y;
        protected override float GetVariablePosition(PointF position) => position.X;

        protected override void SetFixedPosition(PositionComp positionComp, float value) =>
            positionComp.SetPosition(y: value);
        protected override void SetVariablePosition(PositionComp positionComp, float value) =>
            positionComp.SetPosition(x: value);

        protected override void ReverseSpeed(MovementComp movementComp, float amortiguationFactor) =>
            movementComp.MultiplySpeed(factorY: amortiguationFactor);
    }
}
