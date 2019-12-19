using Seedwork.Core.Components;

namespace Rogueskiv.Core.Components.Position
{
    public interface IPositionComp : IComponent
    {
        float X { get; set; }
        float Y { get; set; }
        bool Visible { get; set; }
    }
}
