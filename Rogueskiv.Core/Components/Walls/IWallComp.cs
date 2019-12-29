using Rogueskiv.Core.Components.Position;
using Seedwork.Core.Components;

namespace Rogueskiv.Core.Components.Walls
{
    public interface IWallComp : IComponent
    {
        PositionComp PositionComp { get; }
        int Size { get; }

        bool CheckBounce(
            MovementComp movementComp,
            CurrentPositionComp currentPositionComp,
            LastPositionComp lastPositionComp
        );
    }
}
