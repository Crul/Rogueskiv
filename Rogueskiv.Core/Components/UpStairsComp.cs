using Rogueskiv.Core.GameEvents;
using System.Drawing;

namespace Rogueskiv.Core.Components
{
    public class UpStairsComp : StairsComp
    {
        public UpStairsComp(Point tilePos) : base(tilePos)
        { }

        public override IGameEvent GetGameEvent() => new StairsUpEvent();
    }
}
