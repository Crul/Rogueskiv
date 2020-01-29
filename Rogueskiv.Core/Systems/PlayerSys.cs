using Rogueskiv.Core.Components;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using Seedwork.Crosscutting;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    public class PlayerSys : BaseSystem
    {
        private readonly float Acceleration;
        private PlayerComp PlayerComp;
        private MovementComp PlayerMovementComp;

        public PlayerSys(float playerAcceleration) =>
            Acceleration = playerAcceleration;

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
            if (controls.Any(c => c == (int)Controls.UP)) speedY = -Acceleration;
            if (controls.Any(c => c == (int)Controls.DOWN)) speedY = Acceleration;
            if (controls.Any(c => c == (int)Controls.LEFT)) speedX = -Acceleration;
            if (controls.Any(c => c == (int)Controls.RIGHT)) speedX = Acceleration;

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
