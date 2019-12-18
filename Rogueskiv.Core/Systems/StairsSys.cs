using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Position;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    class StairsSys : BaseSystem
    {
        private const int TILE_SIZE = 30; // TODO proper tile size

        private Game Game;
        private StairsComp StairsComp;
        private PositionComp PlayerPositionComp;

        public override bool Init(Game game)
        {
            Game = game;

            StairsComp = game
                .Entities
                .GetWithComponent<StairsComp>()
                .Single()
                .GetComponent<StairsComp>();

            PlayerPositionComp = game
                .Entities
                .GetWithComponent<PlayerComp>()
                .Single()
                .GetComponent<CurrentPositionComp>();

            return base.Init(game);
        }

        public override void Update(EntityList entities, IEnumerable<int> controls)
        {
            if (
                PlayerPositionComp.X > StairsComp.X - (TILE_SIZE / 2)
                && PlayerPositionComp.X < StairsComp.X + (TILE_SIZE / 2)
                && PlayerPositionComp.Y > StairsComp.Y - (TILE_SIZE / 2)
                && PlayerPositionComp.Y < StairsComp.Y + (TILE_SIZE / 2)
            )
                Game.EndGame(RogueskivGameResults.FloorDown);
        }
    }
}
