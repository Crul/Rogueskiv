using Rogueskiv.Core.Components.Position;
using System.Drawing;

namespace Rogueskiv.Core.Components
{
    public abstract class StairsComp : TilePositionComp
    {
        protected StairsComp(Point tilePos) : base(tilePos)
        { }
    }
}
