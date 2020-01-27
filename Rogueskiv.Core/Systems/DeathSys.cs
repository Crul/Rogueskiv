using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.GameEvents;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    class DeathSys : BaseSystem
    {
        private RogueskivGame Game;
        private HealthComp PlayerHealthComp;
        private CurrentPositionComp CurrentPositionComp;
        private LastPositionComp LastPositionComp;

        public override void Init(Game game)
        {
            Game = (RogueskivGame)game;
            var playerComp = Game.Entities.GetWithComponent<PlayerComp>().Single();
            PlayerHealthComp = playerComp.GetComponent<HealthComp>();
            CurrentPositionComp = playerComp.GetComponent<CurrentPositionComp>();
            LastPositionComp = playerComp.GetComponent<LastPositionComp>();
        }

        public override void Update(EntityList entities, List<int> controls)
        {
            if (PlayerHealthComp.Health > 0)
                return;

            LastPositionComp.Position = CurrentPositionComp.Position;
            Game.GameEvents.Add(new DeathEvent());
            Game.EndGame(RogueskivGameResults.DeathResult, pauseBeforeQuit: true);
        }
    }
}
