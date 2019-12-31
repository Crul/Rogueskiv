using Rogueskiv.Core.GameEvents;
using System.Drawing;

namespace Rogueskiv.Core.Components
{
    public class AmuletComp : PickableComp
    {
        public AmuletComp(int pickingTime, Point tilePos)
            : base(pickingTime, tilePos) { }

        public override IGameEvent GetGameEvent() => null;
    }
}
