using Rogueskiv.Core;
using Rogueskiv.Core.GameEvents;

namespace Rogueskiv.Ux.EffectPlayers
{
    class FoodPickedEffectPlayer : EventEffectPlayer<FoodPickedEvent>
    {
        public FoodPickedEffectPlayer(RogueskivGame game, int channel = -1)
            : base(game, "food_picked", channel)
        { }

        protected override int GetVolume(FoodPickedEvent gameEvent) =>
            base.GetVolume(gameEvent) / 3;
    }
}
