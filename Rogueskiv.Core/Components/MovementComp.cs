using Seedwork.Core.Components;
using Seedwork.Crosscutting;
using System;
using System.Drawing;

namespace Rogueskiv.Core.Components
{
    public class MovementComp : IComponent
    {
        // set in PlayerSys (TODO ?)
        public static float MAX_POS_SPEED;
        public static float MAX_NEG_SPEED;
        public static float STOP_ABS_SPEED;

        private PointF speed;
        public PointF Speed
        {
            get => speed;
            set => speed = value.Map(BoundSpeed);
        }

        public int Radius { get; }
        public float FrictionFactor { get; set; }
        public float BounceAmortiguationFactor { get; set; }
        public bool SimpleBounce { get; }

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

        private static float BoundSpeed(float speed)
        {
            var boundedSpeed = Math.Max(MAX_NEG_SPEED, Math.Min(MAX_POS_SPEED, speed));

            return (Math.Abs(boundedSpeed) < STOP_ABS_SPEED ? 0 : boundedSpeed);
        }
    }
}
