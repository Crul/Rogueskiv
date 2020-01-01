using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.GameEvents;
using System.Drawing;

namespace Rogueskiv.Core.Components
{
    public abstract class PickableComp : CurrentPositionComp
    {
        public override bool AllowRevealedByMap => false;
        public bool IsBeingPicked { get; private set; } = false;
        public int PickingTime { get; private set; }
        public int MaxPickingTime { get; set; }

        protected PickableComp(int maxPickingTime, Point tilePos)
            : base(tilePos) =>
            MaxPickingTime = maxPickingTime;

        public void StartPicking(int pickingTime)
        {
            IsBeingPicked = true;
            PickingTime = pickingTime;
        }

        public void TickPickingTime() => PickingTime--;

        public abstract IGameEvent GetGameEvent();
    }
}
