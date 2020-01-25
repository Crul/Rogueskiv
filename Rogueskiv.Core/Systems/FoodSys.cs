using Rogueskiv.Core.Components;
using Seedwork.Core;
using Seedwork.Core.Entities;
using System.Collections.Generic;

namespace Rogueskiv.Core.Systems
{
    class FoodSys : PickingSys<FoodComp>
    {
        private readonly int FoodHealthIncrease;

        private HealthComp PlayerHealthComp;

        public FoodSys(int foodHealthIncrease) : base(isSingleCompPerFloor: true)
            => FoodHealthIncrease = foodHealthIncrease;

        public override void Init(Game game)
        {
            base.Init(game);

            PlayerHealthComp = game
                .Entities
                .GetSingleComponent<PlayerComp, HealthComp>();
        }

        protected override bool CanIPick() => !PlayerHealthComp.Full;

        protected override void StartPicking(List<FoodComp> pickedFoods)
        {
            base.StartPicking(pickedFoods);
            PlayerHealthComp.Health += FoodHealthIncrease * pickedFoods.Count;
        }
    }
}
