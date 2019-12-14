using Seedwork.Core.Components;

namespace Rogueskiv.Core.Components
{
    public class MovementComp : IComponent
    {
        public float SpeedX { get; set; }
        public float SpeedY { get; set; }

        public float FrictionFactor { get; set; }
        public float BounceAmortiguationFactor { get; set; }
    }
}
