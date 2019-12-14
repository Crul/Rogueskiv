using Rogueskiv.Core.Components.Position;
using Seedwork.Core.Components;

namespace Rogueskiv.Core.Components.Walls
{
    public interface IWallComp : IComponent
    {
        PositionComp Position { get; }
        int Size { get; }

        bool CheckBounce(
            MovementComp movement,
            PositionComp position,
            PositionComp oldPosition
        );
    }
}
