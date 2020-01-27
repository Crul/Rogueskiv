using Rogueskiv.Core.Components;
using Seedwork.Core;
using Seedwork.Core.Entities;
using System.Collections.Generic;

namespace Rogueskiv.Core.Systems
{
    class TorchSys : PickingSys<TorchComp>
    {
        private readonly float TorchVisualRangeIncrease;
        private PlayerComp PlayerComp;
        private FOVComp FOVComp;

        public TorchSys(float torchVisualRangeIncrease) : base(isSingleCompPerFloor: true)
            => TorchVisualRangeIncrease = torchVisualRangeIncrease;

        public override void Init(Game game)
        {
            base.Init(game);

            PlayerComp = game.Entities.GetSingleComponent<PlayerComp>();
            FOVComp = game.Entities.GetSingleComponent<FOVComp>();
        }

        protected override void StartPicking(List<TorchComp> pickedTorchs)
        {
            base.StartPicking(pickedTorchs);
            PlayerComp.VisualRange += TorchVisualRangeIncrease * pickedTorchs.Count;
            FOVComp.SetVisualRange(PlayerComp);
        }
    }
}
