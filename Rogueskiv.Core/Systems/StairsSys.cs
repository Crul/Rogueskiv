using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using Seedwork.Crosscutting;
using Seedwork.Engine;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    class StairsSys : BaseSystem
    {
        private RogueskivGame Game;
        private UpStairsComp UpStairsComp;
        private DownStairsComp DownStairsComp;
        private IEntity PlayerEntity;
        private PositionComp PlayerPositionComp;
        private bool HasExitedStairs = false;

        public override void Init(Game game)
        {
            Game = (RogueskivGame)game;

            var stairsComps = game.Entities.GetComponents<StairsComp>();
            UpStairsComp = (UpStairsComp)stairsComps.FirstOrDefault(s => s is UpStairsComp);
            DownStairsComp = (DownStairsComp)stairsComps.FirstOrDefault(s => s is DownStairsComp);

            PlayerEntity = game
                .Entities
                .GetWithComponent<PlayerComp>()
                .Single();

            PlayerPositionComp = PlayerEntity.GetComponent<CurrentPositionComp>();
        }

        public override void Update(EntityList entities, List<int> controls)
        {
            var isInDownStairs = IsInStairs(DownStairsComp);
            if (isInDownStairs && HasExitedStairs)
            {
                EndGame(RogueskivGameResults.FloorDown, DownStairsComp);
                return;
            }

            var isInUpStairs = IsInStairs(UpStairsComp);
            if (isInUpStairs && HasExitedStairs)
            {
                EndGame(RogueskivGameResults.FloorUp, UpStairsComp);
                return;
            }

            if (!isInDownStairs && !isInUpStairs)
                HasExitedStairs = true;
        }

        private void EndGame(
            IGameResult<EntityList> gameresult, StairsComp stairsComp, bool pauseBeforeQuit = false
        )
        {
            // reset for next floor execution
            HasExitedStairs = false;
            PlayerPositionComp.Position = stairsComp.Position.Add(BoardComp.TILE_SIZE / 2);

            gameresult.Data.Clear();
            gameresult.Data.Add(PlayerEntity.Id, PlayerEntity);

            Game.GameEvents.Add(stairsComp.GetGameEvent());
            Game.EndGame(gameresult, pauseBeforeQuit);
        }

        private bool IsInStairs(StairsComp stairsComp) =>
            stairsComp != null && PlayerPositionComp.TilePos == stairsComp.TilePos;
    }
}
