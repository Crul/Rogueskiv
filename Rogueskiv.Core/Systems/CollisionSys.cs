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
        private const int COLLISION_DAMAGE = 5;

        private BoardComp BoardComp;
        private EntityId PlayerId;
        private CurrentPositionComp PlayerPos;
        private HealthComp PlayerHealth;

        public override void Init(Game game)
        {
            BoardComp = game
                .Entities
                .GetWithComponent<BoardComp>()
                .Single()
                .GetComponent<BoardComp>();


            var player = game
                .Entities
                .GetWithComponent<PlayerComp>()
                .Single();

            PlayerId = player.Id;

            PlayerPos = player.GetComponent<CurrentPositionComp>();
            PlayerHealth = player.GetComponent<HealthComp>();
        }

        public override void Update(EntityList entities, IEnumerable<int> controls) =>
            PlayerHealth.Health -= GetDamage(entities);

        private int GetDamage(EntityList entities) =>
            COLLISION_DAMAGE * BoardComp
                .GetEntityIdsNear(PlayerId, PlayerPos)
                .Select(id => entities[id])
                .Where(enemy =>
                {
                    var enemyPos = enemy.GetComponent<CurrentPositionComp>();
                    var distance = Distance.Get(enemyPos.X - PlayerPos.X, enemyPos.Y - PlayerPos.Y);
                    return (distance < COLLISION_DISTANCE);
                })
                .Count();
    }
}
