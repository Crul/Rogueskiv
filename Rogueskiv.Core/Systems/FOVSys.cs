using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    class FOVSys : BaseSystem
    {
        private Game Game;
        private PlayerComp PlayerComp;
        private CurrentPositionComp PlayerPosComp;
        private FOVComp FOVComp;

        public override void Init(Game game)
        {
            Game = game;

            var playerEntity = Game
                .Entities
                .GetWithComponent<PlayerComp>()
                .Single();

            PlayerComp = playerEntity.GetComponent<PlayerComp>();
            PlayerPosComp = playerEntity.GetComponent<CurrentPositionComp>();

            FOVComp = Game.Entities.GetSingleComponent<FOVComp>();
            var boardComp = Game.Entities.GetSingleComponent<BoardComp>();

            FOVComp.Init(boardComp, PlayerComp);
        }

        public override void Update(EntityList entities, List<int> controls) =>
            FOVComp.SetPlayerPos(PlayerComp, PlayerPosComp);
    }
}
