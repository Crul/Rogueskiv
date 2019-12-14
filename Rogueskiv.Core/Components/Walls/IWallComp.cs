using Rogueskiv.Core.Components.Position;
using Seedwork.Core.Components;

namespace Rogueskiv.Core.Components.Walls
{
    interface IWallComp : IComponent
    {
        bool CheckBounce(
            MovementComp movement,
            PositionComp position,
            PositionComp oldPosition
        );
    }
}
