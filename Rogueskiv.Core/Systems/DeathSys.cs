﻿using Rogueskiv.Core.Components;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    class DeathSys : BaseSystem
    {
        private Game Game;
        private HealthComp PlayerHealth;

        public override bool Init(Game game)
        {
            Game = game;

            PlayerHealth = Game
                .Entities
                .GetWithComponent<PlayerComp>()
                .Single()
                .GetComponent<HealthComp>();

            return base.Init(game);
        }
        public override void Update(EntityList entities, IEnumerable<int> controls)
        {
            if (PlayerHealth.Health < 0)
                Game.Pause = true; // TODO implementDeath
        }
    }
}
