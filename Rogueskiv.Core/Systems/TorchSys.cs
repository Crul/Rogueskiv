using Rogueskiv.Core.Components;
using Seedwork.Core;
using Seedwork.Core.Entities;
using System.Collections.Generic;

namespace Rogueskiv.Core.Systems
{
    class TorchSys : PickingSys<TorchComp>
    {
        private PlayerComp PlayerComp;

        public TorchSys() : base(isSingleCompPerFloor: true) { }

        public override void Init(Game game)
        {
            base.Init(game);

            PlayerComp = game
                .Entities
                .GetSingleComponent<PlayerComp>();
        }

        protected override void StartPicking(List<TorchComp> pickedTorchs)
        {
            base.StartPicking(pickedTorchs);
            PlayerComp.VisualRange += TorchComp.VISUAL_RANGE_INCREASE * pickedTorchs.Count;
        }
    }
}
