using Rogueskiv.Core.Components;
using Rogueskiv.Core.Controls;
using Rogueskiv.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    public class PlayerSys
    {
        private const float ACCELERATION = 6;
        private const float MAX_POS_SPEED = 28;
        private const float MAX_NEG_SPEED = -MAX_POS_SPEED;
        private const float STOP_SPEED = 1;

        public void Update(
            IList<IEntity> entities,
            IEnumerable<Control> controls
        )
        {
            // TODO proper inertia (using angle)

            float speedX = 0, speedY = 0;
            if (controls.Any(c => c == Control.UP)) speedY = -ACCELERATION;
            if (controls.Any(c => c == Control.DOWN)) speedY = ACCELERATION;
            if (controls.Any(c => c == Control.LEFT)) speedX = -ACCELERATION;
            if (controls.Any(c => c == Control.RIGHT)) speedX = ACCELERATION;

            var player = entities.Single(e => e.HasComponent<PlayerComp>());
            var movementComp = player.GetComponent<MovementComp>();
            if (speedX == 0)
                speedX = -movementComp.SpeedX / 3;

            if (speedY == 0)
                speedY = -movementComp.SpeedY / 3;

            movementComp.SpeedX = BoundSpeed(movementComp.SpeedX + speedX);
            movementComp.SpeedY = BoundSpeed(movementComp.SpeedY + speedY);
        }

        private static float BoundSpeed(float speed)
        {
            var boundedSpeed = Math.Max(MAX_NEG_SPEED, Math.Min(MAX_POS_SPEED, speed));

            return (Math.Abs(boundedSpeed) < STOP_SPEED ? 0 : boundedSpeed);
        }
    }
}
