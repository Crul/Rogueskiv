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
        private Game Game;
        private CurrentPositionComp PlayerPosComp;
        private FOVComp FOVComp;
        private BoardComp BoardComp;
        private List<TileComp> TileComps;

        public override void Init(Game game)
        {
            Game = game;

            var playerEntity = Game
                .Entities
                .GetWithComponent<PlayerComp>()
                .Single();

            PlayerPosComp = playerEntity.GetComponent<CurrentPositionComp>();

            FOVComp = Game.Entities.GetSingleComponent<FOVComp>();
            TileComps = Game.Entities.GetComponents<TileComp>();
            BoardComp = Game.Entities.GetSingleComponent<BoardComp>();

            var playerComp = playerEntity.GetComponent<PlayerComp>();
            FOVComp.Init(BoardComp, playerComp);
        }

        public override void Update(EntityList entities, List<int> controls)
        {
            FOVComp.SetPlayerPos(PlayerPosComp);

            var otherPositions = entities.GetComponents<CurrentPositionComp>();

            TileComps
                .Select(t => (PositionComp)t)
                .Concat(otherPositions)
                .ToList()
                .ForEach(comp => comp.Visible = FOVComp.IsVisible(comp));

            FOVComp.Reset();
            TileComps.ForEach(SetFOVInfo);
        }

        private void SetFOVInfo(TileComp tileComp)
        {
            (int tileX, int tileY) = (tileComp.TilePos.X, tileComp.TilePos.Y);

            var tileFOVInfo = FOVComp.GetTileFOVInfo(tileX, tileY);
            tileFOVInfo.Hidden = tileComp.Visible && !tileComp.VisibleByPlayer;
            tileFOVInfo.VisibleByPlayer = tileComp.VisibleByPlayer;
            tileFOVInfo.DistanceFromPlayer = Distance.Get(tileComp.Position, PlayerPosComp.Position);
        }
    }
}
