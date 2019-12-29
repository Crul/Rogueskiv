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
        private CurrentPositionComp PlayerPosComp;
        private HealthComp PlayerHealthComp;

        public override void Init(Game game)
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

            PlayerPosComp = player.GetComponent<CurrentPositionComp>();
            PlayerHealthComp = player.GetComponent<HealthComp>();
        }

        public override void Update(EntityList entities, List<int> controls)
        {
            var collidedEntityIds = GetCollidedEntityIds(entities);
            collidedEntityIds.ForEach(Game.RemoveEntity);

            PlayerHealthComp.Health -= COLLISION_DAMAGE * collidedEntityIds.Count;
        }

        private List<EntityId> GetCollidedEntityIds(EntityList entities) =>
            BoardComp
                .GetEntityIdsNear(PlayerId, PlayerPosComp)
                .Where(id =>
                {
                    if (!entities.ContainsKey(id)) // TODO debug corner case
                        return false;

                    var positionComp = entities[id].GetComponent<CurrentPositionComp>();
                    var distance = Distance.Get(positionComp.Position, PlayerPosComp.Position);
                    return (distance < COLLISION_DISTANCE);
                })
                .ToList();
    }
}
