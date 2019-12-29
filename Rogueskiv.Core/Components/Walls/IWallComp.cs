using Rogueskiv.Core.Components.Position;
using System.Collections.Generic;
using System.Drawing;

namespace Rogueskiv.Core.Components.Walls
{
    public interface IWallComp : IPositionComp
    {
        int Size { get; }

        bool CheckBounce(
            MovementComp movementComp,
            CurrentPositionComp currentPositionComp,
            LastPositionComp lastPositionComp
        );

        List<Point> GetTiles();
    }
}
