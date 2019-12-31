using Seedwork.Core.Components;
using Seedwork.Crosscutting;
using System.Drawing;

namespace Rogueskiv.Core.Components
{
    public class MovementComp : IComponent
    {
        public virtual PointF Speed { get; set; }
        public int Radius { get; }
        public float FrictionFactor { get; }
        public float BounceAmortiguationFactor { get; }
        public bool SimpleBounce { get; }
        public bool HasBounced { get; set; }

        public MovementComp(
            float frictionFactor,
            float bounceAmortiguationFactor,
            int radius,
            PointF? speed = null,
            bool simpleBounce = true
        )
        {
            Radius = radius;
            Speed = speed ?? new PointF(0, 0);
            FrictionFactor = frictionFactor;
            BounceAmortiguationFactor = bounceAmortiguationFactor;
            SimpleBounce = simpleBounce;
        }

        public void MultiplySpeed(float factorX = 1, float factorY = 1) =>
            Speed = Speed.Multiply(factorX, factorY);

        public void Stop() => Speed = new PointF(0, 0);
    }
}
