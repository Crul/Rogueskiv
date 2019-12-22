using Seedwork.Core.Components;
using System.Drawing;

namespace Rogueskiv.Core.Components.Position
{
    public interface IPositionComp : IComponent
    {
        PointF Position { get; }
        bool Visible { get; set; }
    }
}
