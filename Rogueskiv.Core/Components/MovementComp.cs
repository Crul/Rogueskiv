using Seedwork.Core.Components;
using Seedwork.Crosscutting;
using System.Drawing;

namespace Rogueskiv.Core.Components
{
    public class MovementComp : IComponent
    {
        public PointF Speed { get; set; }
        public int Radius { get; }

        public float FrictionFactor { get; set; }
        public float BounceAmortiguationFactor { get; set; }

        public MovementComp(
            float frictionFactor,
            float bounceAmortiguationFactor,
            int radius,
            PointF? speed = null
        )
        {
            Radius = radius;
            Speed = speed ?? new PointF(0, 0);
            FrictionFactor = frictionFactor;
            BounceAmortiguationFactor = bounceAmortiguationFactor;
        }

        public void MultiplySpeed(float factorX = 1, float factorY = 1) =>
            Speed = Speed.Multiply(factorX, factorY);

        public void Stop() => Speed = new PointF(0, 0);
    }
}
