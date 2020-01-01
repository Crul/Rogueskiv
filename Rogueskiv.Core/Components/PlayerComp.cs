using Seedwork.Core.Components;
using System.Collections.Generic;

namespace Rogueskiv.Core.Components
{
    public class PlayerComp : IComponent
    {
        public float VisualRange { get; set; }
        public List<PickableComp> PickingComps { get; } = new List<PickableComp>();
    }
}
