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
