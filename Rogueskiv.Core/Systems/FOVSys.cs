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
        private const int VISUAL_RANGE = 10; // TODO proper visual range
        private FOVRecurse FOVRecurse;
        private CurrentPositionComp PlayerPosComp;
        private List<TileComp> TileComps;

        public override bool Init(Game game)
        {
            PlayerPosComp = game
                .Entities
                .GetWithComponent<PlayerComp>()
                .Single()
                .GetComponent<CurrentPositionComp>();

            TileComps = game
                .Entities
                .GetWithComponent<TileComp>()
                .Select(e => e.GetComponent<TileComp>())
                .ToList();

            InitFOV(game);

            return base.Init(game);
        }

        public override void Update(EntityList entities, List<int> controls)
        {
            FOVRecurse.SetPlayerPos(
                (int)(PlayerPosComp.X / BoardComp.TILE_SIZE),
                (int)(PlayerPosComp.Y / BoardComp.TILE_SIZE)
            );

            var otherPositions = entities
                .GetWithComponent<CurrentPositionComp>()
                .Select(e => e.GetComponent<CurrentPositionComp>());

            TileComps
                .Select(t => (PositionComp)t)
                .Concat(otherPositions)
                .ToList()
                .ForEach(SetVisibility);

            TileComps.ForEach(tileComp =>
                tileComp.DistanceFromPlayer = Distance.Get(
                    tileComp.X - PlayerPosComp.X,
                    tileComp.Y - PlayerPosComp.Y
                ));
        }

        private void SetVisibility(PositionComp positionComp)
        {
            var tileX = (int)(positionComp.X / BoardComp.TILE_SIZE);
            var tileY = (int)(positionComp.Y / BoardComp.TILE_SIZE);

            positionComp.Visible = FOVRecurse
                .VisiblePoints
                .Any(vp => vp.X == tileX && vp.Y == tileY);
        }

        private void InitFOV(Game game)
        {
            var boardComp = game
                .Entities
                .GetWithComponent<BoardComp>()
                .Single()
                .GetComponent<BoardComp>();

            (var width, var height) = BoardSys.GetSize(boardComp.Board);

            FOVRecurse = new FOVRecurse(width, height, VISUAL_RANGE);
            BoardSys.ForAllTiles(width, height, (x, y) =>
                FOVRecurse.Point_Set(x, y, !BoardSys.IsTile(boardComp.Board, x, y) ? 1 : 0));
        }
    }
}
