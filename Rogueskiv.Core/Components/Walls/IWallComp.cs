using Rogueskiv.Core.Components.Position;
using Seedwork.Core.Components;
using System.Collections.Generic;

namespace Rogueskiv.Core.Components.Walls
{
    public interface IWallComp : IComponent
    {
        PositionComp Position { get; }
        int Size { get; }
        WallFacingDirections Facing { get; }
        List<WallTile> Tiles { get; }

        bool CheckBounce(
            MovementComp movement,
            PositionComp position,
            PositionComp oldPosition
        );
    }
}
