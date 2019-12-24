
using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Position;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    class TorchSys : BaseSystem
    {
        private Game Game;
        private PositionComp PlayerPositionComp;
        private PlayerComp PlayerComp;

        public override void Init(Game game)
        {
            Game = game;

            var playerEntity = game
                .Entities
                .GetWithComponent<PlayerComp>()
                .Single();

            PlayerPositionComp = playerEntity.GetComponent<CurrentPositionComp>();
            PlayerComp = playerEntity.GetComponent<PlayerComp>();
        }

        public override void Update(EntityList entities, List<int> controls)
        {
            // TODO DRY FoodSys

            var pickedTorchs = entities
                .GetWithComponent<TorchComp>()
                .Select(e => (
                    torchEntityId: e.Id,
                    torchComp: e.GetComponent<TorchComp>()
                ))
                .Where(torch => torch.torchComp.TilePos == PlayerPositionComp.TilePos)
                .ToList();

            if (pickedTorchs.Any())
            {
                PlayerComp.VisualRange += TorchComp.VISUAL_RANGE_INCREASE * pickedTorchs.Count;
                pickedTorchs.ForEach(torch => entities.Remove(torch.torchEntityId));
                Game.RemoveSystem(this);  // only one torch per floor
            }
        }
    }
}
