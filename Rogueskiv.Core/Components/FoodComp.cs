using System.Drawing;

namespace Rogueskiv.Core.Components
{
    public class FoodComp : PickableComp
    {
        public FoodComp(int maxPickingTime, Point tilePos)
            : base(maxPickingTime, tilePos) { }
    }
}
