using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.GameEvents;
using System.Drawing;

namespace Rogueskiv.Core.Components
{
    public abstract class StairsComp : TilePositionComp
    {
        protected StairsComp(Point tilePos) : base(tilePos)
        { }

        public abstract IGameEvent GetGameEvent();
    }
}
