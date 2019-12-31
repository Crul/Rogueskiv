using Rogueskiv.Core.GameEvents;
using System.Drawing;

namespace Rogueskiv.Core.Components
{
    public class MapRevealerComp : PickableComp
    {
        public MapRevealerComp(int maxPickingTime, Point tilePos)
            : base(maxPickingTime, tilePos) { }

        public override IGameEvent GetGameEvent() => new MapRevealerPickedEvent();
    }
}
