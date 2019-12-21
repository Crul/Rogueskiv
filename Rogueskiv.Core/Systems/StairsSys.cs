using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
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
        private Game Game;
        private UpStairsComp UpStairsComp;
        private DownStairsComp DownStairsComp;
        private IEntity PlayerEntity;
        private PositionComp PlayerPositionComp;
        private bool HasExitedStairs = false;
        private readonly bool IsLastFloor;

        public StairsSys(bool isLastFloor) => IsLastFloor = isLastFloor;

        public override bool Init(Game game)
        {
            Game = game;

            var stairsComps = game
                .Entities
                .GetWithComponent<StairsComp>()
                .Select(s => s.GetComponent<StairsComp>());

            UpStairsComp = (UpStairsComp)stairsComps.FirstOrDefault(s => s is UpStairsComp);
            DownStairsComp = (DownStairsComp)stairsComps.Single(s => s is DownStairsComp);

            PlayerEntity = game
                .Entities
                .GetWithComponent<PlayerComp>()
                .Single();

            PlayerPositionComp = PlayerEntity.GetComponent<CurrentPositionComp>();

            return base.Init(game);
        }

        public override void Update(EntityList entities, List<int> controls)
        {
            var isInDownStairs = IsInStairs(DownStairsComp);
            if (isInDownStairs && HasExitedStairs)
            {
                if (IsLastFloor)
                    EndGame(RogueskivGameResults.WinResult, DownStairsComp, pauseBeforeQuit: true);
                else
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

        private void EndGame(
            IGameResult<IEntity> gameresult, StairsComp stairsComp, bool pauseBeforeQuit = false
        )
        {
            // reset for next floor execution
            HasExitedStairs = false;
            PlayerPositionComp.Position = stairsComp.Position;

            gameresult.Data.Clear();
            gameresult.Data.Add(PlayerEntity);

            Game.EndGame(gameresult, pauseBeforeQuit);
        }

        private bool IsInStairs(StairsComp stairsComp) =>
             PlayerPositionComp.Position.X > stairsComp.Position.X - (BoardComp.TILE_SIZE / 2)
            && PlayerPositionComp.Position.X < stairsComp.Position.X + (BoardComp.TILE_SIZE / 2)
            && PlayerPositionComp.Position.Y > stairsComp.Position.Y - (BoardComp.TILE_SIZE / 2)
            && PlayerPositionComp.Position.Y < stairsComp.Position.Y + (BoardComp.TILE_SIZE / 2);
    }
}
