using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using Seedwork.Crosscutting;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    class CollisionSys : BaseSystem
    {
        private const int COLLISION_DISTANCE = 14; // calc from entity sizes
        private const int COLLISION_DAMAGE = 10;

        private Game Game;
        private BoardComp BoardComp;
        private EntityId PlayerId;
        private CurrentPositionComp PlayerPos;
        private HealthComp PlayerHealth;

        public override bool Init(Game game)
        {
            Game = game;

            BoardComp = Game
                .Entities
                .GetWithComponent<BoardComp>()
                .Single()
                .GetComponent<BoardComp>();


            var player = Game
                .Entities
                .GetWithComponent<PlayerComp>()
                .Single();

            PlayerId = player.Id;

            PlayerPos = player.GetComponent<CurrentPositionComp>();
            PlayerHealth = player.GetComponent<HealthComp>();

            return base.Init(game);
        }

        public override void Update(EntityList entities, IEnumerable<int> controls)
        {
            var collidedEntityIds = GetCollidedEntityIds(entities);
            collidedEntityIds.ForEach(Game.RemoveEntity);

            PlayerHealth.Health -= COLLISION_DAMAGE * collidedEntityIds.Count;
        }

        private List<EntityId> GetCollidedEntityIds(EntityList entities) =>
            BoardComp
                .GetEntityIdsNear(PlayerId, PlayerPos)
                .Where(id =>
                {
                    if (!entities.ContainsKey(id)) // TODO debug corner case
                        return false;

                    var position = entities[id].GetComponent<CurrentPositionComp>();
                    var distance = Distance.Get(position.X - PlayerPos.X, position.Y - PlayerPos.Y);
                    return (distance < COLLISION_DISTANCE);
                })
                .ToList();
    }
}
