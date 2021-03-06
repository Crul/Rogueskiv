﻿using Seedwork.Crosscutting;
using System;
using System.Drawing;

namespace Rogueskiv.Core.Components
{
    public class BoundedMovementComp : MovementComp
    {
        public float MaxSpeed { get; }

        private readonly float StopSpeed;
        private PointF speed;

        public override PointF Speed
        {
            get => speed;
            set => speed = value.Map(BoundSpeed);
        }

        public BoundedMovementComp(
            float maxSpeed,
            float stopSpeed,
            float frictionFactor,
            float bounceMomentumConservationFactor,
            int radius,
            PointF? speed = null,
            bool simpleBounce = true
        ) : base(frictionFactor, bounceMomentumConservationFactor, radius, speed, simpleBounce)
        {
            MaxSpeed = maxSpeed;
            StopSpeed = stopSpeed;
        }

        private float BoundSpeed(float speed)
        {
            var boundedSpeed = Math.Max(-MaxSpeed, Math.Min(MaxSpeed, speed));

            return (Math.Abs(boundedSpeed) < StopSpeed ? 0 : boundedSpeed);
        }
    }
}
