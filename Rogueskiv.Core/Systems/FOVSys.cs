using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using Seedwork.Crosscutting;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    class FOVSys : BaseSystem
    {
        private CurrentPositionComp PlayerPosComp;
        private FOVComp FOVComp;
        private List<TileComp> TileComps;

        public override bool Init(Game game)
        {
            PlayerPosComp = game
                .Entities
                .GetWithComponent<PlayerComp>()
                .Single()
                .GetComponent<CurrentPositionComp>();

            FOVComp = game
                .Entities
                .GetWithComponent<FOVComp>()
                .Single()
                .GetComponent<FOVComp>();

            TileComps = game
                .Entities
                .GetWithComponent<TileComp>()
                .Select(e => e.GetComponent<TileComp>())
                .ToList();

            var boardComp = game
                .Entities
                .GetWithComponent<BoardComp>()
                .Single()
                .GetComponent<BoardComp>();

            FOVComp.Init(boardComp);

            return base.Init(game);
        }

        public override void Update(EntityList entities, List<int> controls)
        {
            FOVComp.SetPlayerPos(PlayerPosComp);

            var otherPositions = entities
                .GetWithComponent<CurrentPositionComp>()
                .Select(e => e.GetComponent<CurrentPositionComp>());

            TileComps
                .Select(t => (PositionComp)t)
                .Concat(otherPositions)
                .ToList()
                .ForEach(comp => comp.Visible = FOVComp.IsVisible(comp));

            TileComps.ForEach(tileComp =>
                tileComp.DistanceFromPlayer = Distance.Get(tileComp.Position, PlayerPosComp.Position)
            );
        }
    }
}
