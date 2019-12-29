using Seedwork.Core.Components;
using System.Collections.Generic;

namespace Rogueskiv.Core.Components
{
    public class PlayerComp : IComponent
    {
        public const int PLAYER_RADIUS = 10;
        public const int INITIAL_PLAYER_HEALTH = 100;
        public const int INITIAL_VISUAL_RANGE = 5;

        public int VisualRange { get; set; }
        public List<PickableComp> PickingComps { get; } = new List<PickableComp>();
    }
}
