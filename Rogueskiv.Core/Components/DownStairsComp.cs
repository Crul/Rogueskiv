using Rogueskiv.Core.GameEvents;
using System.Drawing;

namespace Rogueskiv.Core.Components
{
    public class DownStairsComp : StairsComp
    {
        public DownStairsComp(Point tilePos) : base(tilePos)
        { }

        public override IGameEvent GetGameEvent() => new StairsDownEvent();
    }
}
