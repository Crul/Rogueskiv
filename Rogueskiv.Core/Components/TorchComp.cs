using Rogueskiv.Core.Components.Position;
using System.Drawing;

namespace Rogueskiv.Core.Components
{
    public class TorchComp : CurrentPositionComp
    {
        public const int VISUAL_RANGE_INCREASE = 1;

        public TorchComp(PointF position) : base(position)
        { }
    }
}
