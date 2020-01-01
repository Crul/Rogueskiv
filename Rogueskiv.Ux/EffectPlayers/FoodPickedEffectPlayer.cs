using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;

namespace Rogueskiv.Ux.EffectPlayers
{
    class FoodPickedEffectPlayer : EventEffectPlayer<FoodPickedEvent>
    {
        public FoodPickedEffectPlayer(RogueskivGame game) : base(game, "food_picked") { }
    }
}
