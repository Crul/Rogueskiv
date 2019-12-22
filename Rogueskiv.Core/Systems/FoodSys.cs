using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Position;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    class FoodSys : BaseSystem
    {
        private PositionComp PlayerPositionComp;
        private HealthComp PlayerHealthComp;

        public override bool Init(Game game)
        {
            var playerEntity = game
                .Entities
                .GetWithComponent<PlayerComp>()
                .Single();

            PlayerPositionComp = playerEntity.GetComponent<CurrentPositionComp>();
            PlayerHealthComp = playerEntity.GetComponent<HealthComp>();


            return base.Init(game);
        }
        public override void Update(EntityList entities, List<int> controls)
        {
            var pickedFoods = entities
                .GetWithComponent<FoodComp>()
                .Select(e => (
                    foodEntityId: e.Id,
                    foodComp: e.GetComponent<FoodComp>()
                ))
                .Where(food => food.foodComp.TilePos == PlayerPositionComp.TilePos)
                .ToList();

            PlayerHealthComp.Health += FoodComp.HEALTH_INCREASE * pickedFoods.Count;

            pickedFoods.ForEach(food => entities.Remove(food.foodEntityId));
        }
    }
}
