using Seedwork.Core.Components;
using System.Drawing;

namespace Rogueskiv.Core.Components
{
    public class MovementComp : IComponent
    {
        public PointF Speed { get; set; } // TODO private set

        public float FrictionFactor { get; set; }
        public float BounceAmortiguationFactor { get; set; }
    }
}
