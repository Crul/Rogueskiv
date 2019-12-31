using Rogueskiv.Core.GameEvents;
using System.Drawing;

namespace Rogueskiv.Core.Components
{
    public class FoodComp : PickableComp
    {
        public FoodComp(int maxPickingTime, Point tilePos)
            : base(maxPickingTime, tilePos) { }

        public override IGameEvent GetGameEvent() => null;
    }
}
