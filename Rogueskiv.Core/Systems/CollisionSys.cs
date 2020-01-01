using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.GameEvents;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using Seedwork.Crosscutting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    class CollisionSys : BaseSystem
    {
        private readonly int CollisionDamage;
        private readonly float CollisionBounce;

        private RogueskivGame Game;
        private BoardComp BoardComp;
        private EntityId PlayerId;
        private CurrentPositionComp PlayerPosComp;
        private MovementComp PlayerMovementComp;
        private HealthComp PlayerHealthComp;

        public CollisionSys(int collisionDamage, float collisionBounce)
        {
            CollisionDamage = collisionDamage;
            CollisionBounce = collisionBounce;
        }

        public override void Init(Game game)
        {
            Game = (RogueskivGame)game;

            BoardComp = Game.Entities.GetSingleComponent<BoardComp>();

            var player = Game
                .Entities
                .GetWithComponent<PlayerComp>()
                .Single();

            PlayerId = player.Id;

            PlayerPosComp = player.GetComponent<CurrentPositionComp>();
            PlayerHealthComp = player.GetComponent<HealthComp>();
            PlayerMovementComp = player.GetComponent<MovementComp>();
        }

        public override void Update(EntityList entities, List<int> controls)
        {
            var collidedEntityIds = GetCollisionsInfo(entities);
            collidedEntityIds.ForEach(colInfo => Game.RemoveEntity(colInfo.entityId));

            if (collidedEntityIds.Count == 0)
                return;

            PlayerHealthComp.Health -= CollisionDamage * collidedEntityIds.Count;

            var speedChangeXSign = Math.Sign(collidedEntityIds.Sum(colInfo => colInfo.bounce.X));
            var speedChangeYSign = Math.Sign(collidedEntityIds.Sum(colInfo => colInfo.bounce.Y));
            var speedChangeX = CollisionBounce * speedChangeXSign;
            var speedChangeY = CollisionBounce * speedChangeYSign;

            PlayerSys.AddSped(PlayerMovementComp, speedChangeX, speedChangeY);

            Game.GameEvents.Add(new EnemyCollidedEvent());
        }

        private List<(EntityId entityId, PointF bounce)> GetCollisionsInfo(EntityList entities) =>
            BoardComp
                .GetEntityIdsNear(PlayerId, PlayerPosComp)
                .Where(id => entities.ContainsKey(id)) // TODO debug corner case when enemy dies?
                .Select(id => entities[id])
                .Select(entity => (
                    entityId: entity.Id,
                    positionComp: entity.GetComponent<CurrentPositionComp>(),
                    movementComp: entity.GetComponent<MovementComp>()
                ))
                .Where(info =>
                {
                    var distance = Distance.Get(info.positionComp.Position, PlayerPosComp.Position);
                    return (distance < PlayerMovementComp.Radius + info.movementComp.Radius);
                })
                .Select(info => (
                    info.entityId,
                    bounce: PlayerPosComp.Position.Substract(info.positionComp.Position)
                ))
                .ToList();
    }
}
