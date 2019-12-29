using System.Drawing;

namespace Rogueskiv.Core.Components
{
    public class TorchComp : PickableComp
    {
        public TorchComp(int maxPickingTime, Point tilePos)
            : base(maxPickingTime, tilePos) { }
    }
}
