using Rogueskiv.Core.Components.Position;
using System.Drawing;

namespace Rogueskiv.Core.Components
{
    public class PickableComp : CurrentPositionComp
    {
        public override bool AllowRevealedByMap => false;
        public bool IsBeingPicked { get; private set; } = false;
        public int PickingTime { get; private set; }
        public int MaxPickingTime { get; set; }

        public PickableComp(int maxPickingTime, Point tilePos)
            : base(tilePos) =>
            MaxPickingTime = maxPickingTime;

        public void StartPicking(int pickingTime)
        {
            IsBeingPicked = true;
            PickingTime = pickingTime;
        }

        public void TickPickingTime() => PickingTime--;
    }
}
