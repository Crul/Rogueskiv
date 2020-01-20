using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;
using Seedwork.Ux;

namespace Rogueskiv.Ux.EffectPlayers
{
    class FoodPickedEffectPlayer : EventEffectPlayer<FoodPickedEvent>
    {
        public FoodPickedEffectPlayer(UxContext uxContext, RogueskivGame game)
            : base(uxContext, game, "food_picked") { }
    }
}
