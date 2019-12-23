using Rogueskiv.Core.Components;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using System.Collections.Generic;

namespace Rogueskiv.Core.Systems
{
    class DeathSys : BaseSystem
    {
        private Game Game;
        private HealthComp PlayerHealthComp;

        public override void Init(Game game)
        {
            Game = game;
            PlayerHealthComp = Game.Entities.GetSingleComponent<HealthComp>();
        }

        public override void Update(EntityList entities, List<int> controls)
        {
            if (PlayerHealthComp.Health > 0)
                return;

            if (Game.Result?.ResultCode == RogueskivGameResults.DeathResult.ResultCode)
                Game.EndGame(Game.Result);
            else
                Game.EndGame(RogueskivGameResults.DeathResult, pauseBeforeQuit: true);
        }
    }
}
