using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Position;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using Seedwork.Engine;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    class StairsSys : BaseSystem
    {
        private const int TILE_SIZE = 30; // TODO proper tile size

        private Game Game;
        private UpStairsComp UpStairsComp;
        private DownStairsComp DownStairsComp;
        private PositionComp PlayerPositionComp;
        private bool HasExitedStairs = false;

        public override bool Init(Game game)
        {
            Game = game;

            var stairsComps = game
                .Entities
                .GetWithComponent<StairsComp>()
                .Select(s => s.GetComponent<StairsComp>());

            UpStairsComp = (UpStairsComp)stairsComps.FirstOrDefault(s => s is UpStairsComp);
            DownStairsComp = (DownStairsComp)stairsComps.Single(s => s is DownStairsComp);

            PlayerPositionComp = game
                .Entities
                .GetWithComponent<PlayerComp>()
                .Single()
                .GetComponent<CurrentPositionComp>();

            return base.Init(game);
        }

        public override void Update(EntityList entities, IEnumerable<int> controls)
        {
            var isInDownStairs = IsInStairs(DownStairsComp);
            if (isInDownStairs && HasExitedStairs)
            {
                EndGame(RogueskivGameResults.FloorDown, DownStairsComp);
                return;
            }

            var isInUpStairs = UpStairsComp == null ? false : IsInStairs(UpStairsComp);
            if (isInUpStairs && HasExitedStairs)
            {
                EndGame(RogueskivGameResults.FloorUp, UpStairsComp);
                return;
            }

            if (!isInDownStairs && !isInUpStairs)
                HasExitedStairs = true;
        }

        private void EndGame(IGameResult gameresult, StairsComp stairsComp)
        {
            // reset for next floor execution
            HasExitedStairs = false;
            PlayerPositionComp.X = stairsComp.X;
            PlayerPositionComp.Y = stairsComp.Y;

            Game.EndGame(gameresult);
        }

        private bool IsInStairs(StairsComp stairsComp) =>
             PlayerPositionComp.X > stairsComp.X - (TILE_SIZE / 2)
            && PlayerPositionComp.X < stairsComp.X + (TILE_SIZE / 2)
            && PlayerPositionComp.Y > stairsComp.Y - (TILE_SIZE / 2)
            && PlayerPositionComp.Y < stairsComp.Y + (TILE_SIZE / 2);
    }
}
