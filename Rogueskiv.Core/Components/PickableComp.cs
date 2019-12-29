using Rogueskiv.Core.Components.Position;
using System.Drawing;

namespace Rogueskiv.Core.Components
{
    public class PickableComp : CurrentPositionComp
    {
        public const int MAX_PICKING_TIME = 20;  // TODO calculate in terms of GameFPS

        public bool IsBeingPicked { get; private set; } = false;
        public int PickingTime { get; private set; }

        public PickableComp(PointF position) : base(position)
        { }

        public void StartPicking()
        {
            IsBeingPicked = true;
            PickingTime = MAX_PICKING_TIME;
        }

        public void TickPickingTime() => PickingTime--;
    }
}
