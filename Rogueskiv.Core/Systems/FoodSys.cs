using Rogueskiv.Core.Components;
using Seedwork.Core;
using Seedwork.Core.Entities;
using System.Collections.Generic;

namespace Rogueskiv.Core.Systems
{
    class FoodSys : PickingSys<FoodComp>
    {
        private HealthComp PlayerHealthComp;

        public FoodSys() : base(isSingleCompPerFloor: true) { }

        public override void Init(Game game)
        {
            base.Init(game);

            PlayerHealthComp = game
                .Entities
                .GetSingleComponent<PlayerComp, HealthComp>();
        }

        protected override bool CanIPick() => !PlayerHealthComp.Full;

        protected override void OnPicked(List<FoodComp> pickedFoods) =>
            PlayerHealthComp.Health += FoodComp.HEALTH_INCREASE * pickedFoods.Count;
    }
}
