using System.Drawing;

namespace Rogueskiv.Core.Components
{
    public class FoodComp : PickableComp
    {
        public const int HEALTH_INCREASE = 30;

        public FoodComp(PointF position) : base(position)
        { }
    }
}
