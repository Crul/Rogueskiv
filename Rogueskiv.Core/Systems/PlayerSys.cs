using Rogueskiv.Core.Components;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using Seedwork.Crosscutting;
using Seedwork.Engine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    public class PlayerSys : BaseSystem
    {
        private static float ACCELERATION;
        private PlayerComp PlayerComp;
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
            MovementComp.MAX_POS_SPEED = 200f / fps;
            MovementComp.MAX_NEG_SPEED = -MovementComp.MAX_POS_SPEED;
            MovementComp.STOP_ABS_SPEED = 1f / fps;
        }

        public override void Init(Game game)
        {
            var playerEntity = game.Entities.GetWithComponent<PlayerComp>().Single();
            PlayerComp = playerEntity.GetComponent<PlayerComp>();
            PlayerMovementComp = playerEntity.GetComponent<MovementComp>();
        }

        public override void Update(EntityList entities, List<int> controls)
        {
            // TODO proper inertia (using angle)

            if (PlayerComp.PickingComps.Any())
            {
                PlayerMovementComp.Stop();
                return;
            }

            float speedX = 0, speedY = 0;
            if (controls.Any(c => c == (int)Controls.UP)) speedY = -ACCELERATION;
            if (controls.Any(c => c == (int)Controls.DOWN)) speedY = ACCELERATION;
            if (controls.Any(c => c == (int)Controls.LEFT)) speedX = -ACCELERATION;
            if (controls.Any(c => c == (int)Controls.RIGHT)) speedX = ACCELERATION;

            if (speedX == 0)
                speedX = -PlayerMovementComp.FrictionFactor * PlayerMovementComp.Speed.X;

            if (speedY == 0)
                speedY = -PlayerMovementComp.FrictionFactor * PlayerMovementComp.Speed.Y;

            AddSped(PlayerMovementComp, speedX, speedY);
        }

        public static void AddSped(MovementComp movementComp, float speedX, float speedY) =>
            movementComp.Speed = movementComp.Speed.Add(speedX, speedY);
    }
}
