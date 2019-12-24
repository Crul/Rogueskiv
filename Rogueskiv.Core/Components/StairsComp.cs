using Rogueskiv.Core.Components.Position;
using System.Drawing;

namespace Rogueskiv.Core.Components
{
    public abstract class StairsComp : RevealableByMapPositionComp
    {
        protected StairsComp(PointF position) : base(position)
        { }
    }
}
