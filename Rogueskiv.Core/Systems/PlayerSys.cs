using Rogueskiv.Core.Components;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using Seedwork.Engine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    public class PlayerSys : BaseSystem
    {
        private static float ACCELERATION;
        private static float MAX_POS_SPEED;
        private static float MAX_NEG_SPEED;
        private static float STOP_ABS_SPEED;
        private MovementComp PlayerMovementComp;

        public PlayerSys(IGameContext gameContext)
        {
            // for 25 FPS:
            //    ACCELERATION = 6 pixels per tick (per tick)
            //    MAX_POS_SPEED / MAX_NEG_SPEED = +/-28 pixels per tick
            //    STOP_ABS_SPEED = 1 pixel per tick

            // the parametrization is not perfect because with high FPS
            // there are more steps with movement for the same acceleration

            var fps = gameContext.GameFPS;
            ACCELERATION = (float)Math.Pow(2d, 25d / fps);
            MAX_POS_SPEED = 200f / fps;
            MAX_NEG_SPEED = -MAX_POS_SPEED;
            STOP_ABS_SPEED = 1f / fps;
        }

        public override bool Init(Game game)
        {
            PlayerMovementComp = game
                .Entities
                .GetWithComponent<PlayerComp>()
                .Single()
                .GetComponent<MovementComp>();

            return base.Init(game);
        }

        public override void Update(EntityList entities, List<int> controls)
        {
            // TODO proper inertia (using angle)

            float speedX = 0, speedY = 0;
            if (controls.Any(c => c == (int)Controls.UP)) speedY = -ACCELERATION;
            if (controls.Any(c => c == (int)Controls.DOWN)) speedY = ACCELERATION;
            if (controls.Any(c => c == (int)Controls.LEFT)) speedX = -ACCELERATION;
            if (controls.Any(c => c == (int)Controls.RIGHT)) speedX = ACCELERATION;

            if (speedX == 0)
                speedX = -PlayerMovementComp.FrictionFactor * PlayerMovementComp.SpeedX;

            if (speedY == 0)
                speedY = -PlayerMovementComp.FrictionFactor * PlayerMovementComp.SpeedY;

            PlayerMovementComp.SpeedX = BoundSpeed(PlayerMovementComp.SpeedX + speedX);
            PlayerMovementComp.SpeedY = BoundSpeed(PlayerMovementComp.SpeedY + speedY);
        }

        private static float BoundSpeed(float speed)
        {
            var boundedSpeed = Math.Max(MAX_NEG_SPEED, Math.Min(MAX_POS_SPEED, speed));

            return (Math.Abs(boundedSpeed) < STOP_ABS_SPEED ? 0 : boundedSpeed);
        }
    }
}
