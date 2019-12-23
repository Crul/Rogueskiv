using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.Components.Walls;
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

            PlayerPosComp = Game
                .Entities
                .GetSingleComponent<PlayerComp, CurrentPositionComp>();

            FOVComp = Game.Entities.GetSingleComponent<FOVComp>();
            TileComps = Game.Entities.GetComponents<TileComp>();
            BoardComp = Game.Entities.GetSingleComponent<BoardComp>();

            FOVComp.Init(BoardComp);
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

            var wallFacingDirections = BoardComp
                .WallsByTiles[tileComp.TilePos]
                .Select(wallId => Game.Entities[wallId].GetComponent<IWallComp>().Facing)
                .ToList();

            var hasWallFacingLeft = wallFacingDirections.Contains(WallFacingDirections.LEFT);
            var hasWallFacingRight = wallFacingDirections.Contains(WallFacingDirections.RIGHT);
            var hasWallFacingUp = wallFacingDirections.Contains(WallFacingDirections.UP);
            var hasWallFacingDown = wallFacingDirections.Contains(WallFacingDirections.DOWN);

            if (hasWallFacingLeft)
                CopyFOVInfo(tileFOVInfo, tileX + 1, tileY);

            if (hasWallFacingRight)
                CopyFOVInfo(tileFOVInfo, tileX - 1, tileY);

            if (hasWallFacingUp)
                CopyFOVInfo(tileFOVInfo, tileX, tileY + 1);

            if (hasWallFacingDown)
                CopyFOVInfo(tileFOVInfo, tileX, tileY - 1);

            if (hasWallFacingDown && hasWallFacingRight)
                CopyFOVInfo(tileFOVInfo, tileX - 1, tileY - 1);

            if (hasWallFacingDown && hasWallFacingLeft)
                CopyFOVInfo(tileFOVInfo, tileX + 1, tileY - 1);

            if (hasWallFacingUp && hasWallFacingRight)
                CopyFOVInfo(tileFOVInfo, tileX - 1, tileY + 1);

            if (hasWallFacingUp && hasWallFacingLeft)
                CopyFOVInfo(tileFOVInfo, tileX + 1, tileY + 1);
        }

        private void CopyFOVInfo(TileFOVInfo tileFOVInfo, int targetTileX, int targetTileY)
        {
            var targetTileFOVInfo = FOVComp.GetTileFOVInfo(targetTileX, targetTileY);

            targetTileFOVInfo.VisibleByPlayer = tileFOVInfo.VisibleByPlayer || tileFOVInfo.VisibleByPlayer;
            targetTileFOVInfo.Hidden = targetTileFOVInfo.Hidden || tileFOVInfo.Hidden;

            if (tileFOVInfo.DistanceFromPlayer < targetTileFOVInfo.DistanceFromPlayer)
                targetTileFOVInfo.DistanceFromPlayer = tileFOVInfo.DistanceFromPlayer;
        }
    }
}
