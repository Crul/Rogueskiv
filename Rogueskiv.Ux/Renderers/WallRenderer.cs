using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Walls;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class WallRenderer : BaseItemRenderer<IWallComp>
    {
        private readonly IntPtr Texture;

        private static (int length, int wide) WallSize = (60, 24);

        private readonly IDictionary<
                (WallFacingDirections facing, WallTipTypes initialTip, WallTipTypes finalTip),
                SDL_Rect
            > WallTexutreRects = new Dictionary<
                (WallFacingDirections facing, WallTipTypes initialTip, WallTipTypes finalTip),
                SDL_Rect
            >
            {
                {(WallFacingDirections.UP, WallTipTypes.FLAT, WallTipTypes.FLAT),       new SDL_Rect(){ x = 0,   y = 32, h = WallSize.wide, w = WallSize.length } },
                {(WallFacingDirections.UP, WallTipTypes.FLAT, WallTipTypes.CONVEXE),    new SDL_Rect(){ x = 0,   y = 56, h = WallSize.wide, w = WallSize.length } },
                {(WallFacingDirections.UP, WallTipTypes.FLAT, WallTipTypes.CONCAVE),    new SDL_Rect(){ x = 0,   y = 80, h = WallSize.wide, w = WallSize.length } },
                {(WallFacingDirections.UP, WallTipTypes.CONVEXE, WallTipTypes.FLAT),    new SDL_Rect(){ x = 60,  y = 32, h = WallSize.wide, w = WallSize.length } },
                {(WallFacingDirections.UP, WallTipTypes.CONVEXE, WallTipTypes.CONVEXE), new SDL_Rect(){ x = 60,  y = 56, h = WallSize.wide, w = WallSize.length } },
                {(WallFacingDirections.UP, WallTipTypes.CONVEXE, WallTipTypes.CONCAVE), new SDL_Rect(){ x = 60,  y = 80, h = WallSize.wide, w = WallSize.length } },
                {(WallFacingDirections.UP, WallTipTypes.CONCAVE, WallTipTypes.FLAT),    new SDL_Rect(){ x = 120, y = 32, h = WallSize.wide, w = WallSize.length } },
                {(WallFacingDirections.UP, WallTipTypes.CONCAVE, WallTipTypes.CONVEXE), new SDL_Rect(){ x = 120, y = 56, h = WallSize.wide, w = WallSize.length } },
                {(WallFacingDirections.UP, WallTipTypes.CONCAVE, WallTipTypes.CONCAVE), new SDL_Rect(){ x = 120, y = 80, h = WallSize.wide, w = WallSize.length } },

                {(WallFacingDirections.DOWN, WallTipTypes.FLAT, WallTipTypes.FLAT),       new SDL_Rect(){ x = 0,   y = 104, h = WallSize.wide, w = WallSize.length } },
                {(WallFacingDirections.DOWN, WallTipTypes.CONVEXE, WallTipTypes.FLAT),    new SDL_Rect(){ x = 0,   y = 128, h = WallSize.wide, w = WallSize.length } },
                {(WallFacingDirections.DOWN, WallTipTypes.CONCAVE, WallTipTypes.FLAT),    new SDL_Rect(){ x = 0,   y = 152, h = WallSize.wide, w = WallSize.length } },
                {(WallFacingDirections.DOWN, WallTipTypes.FLAT, WallTipTypes.CONVEXE),    new SDL_Rect(){ x = 60,  y = 104, h = WallSize.wide, w = WallSize.length } },
                {(WallFacingDirections.DOWN, WallTipTypes.CONVEXE, WallTipTypes.CONVEXE), new SDL_Rect(){ x = 60,  y = 128, h = WallSize.wide, w = WallSize.length } },
                {(WallFacingDirections.DOWN, WallTipTypes.CONCAVE, WallTipTypes.CONVEXE), new SDL_Rect(){ x = 60,  y = 152, h = WallSize.wide, w = WallSize.length } },
                {(WallFacingDirections.DOWN, WallTipTypes.FLAT, WallTipTypes.CONCAVE),    new SDL_Rect(){ x = 120, y = 104, h = WallSize.wide, w = WallSize.length } },
                {(WallFacingDirections.DOWN, WallTipTypes.CONVEXE, WallTipTypes.CONCAVE), new SDL_Rect(){ x = 120, y = 128, h = WallSize.wide, w = WallSize.length } },
                {(WallFacingDirections.DOWN, WallTipTypes.CONCAVE, WallTipTypes.CONCAVE), new SDL_Rect(){ x = 120, y = 152, h = WallSize.wide, w = WallSize.length } },

                {(WallFacingDirections.LEFT, WallTipTypes.FLAT, WallTipTypes.FLAT),       new SDL_Rect(){ x = 228, y = 0,   h = WallSize.length, w = WallSize.wide } },

                {(WallFacingDirections.RIGHT, WallTipTypes.FLAT, WallTipTypes.FLAT),       new SDL_Rect(){ x = 300, y = 0,   h = WallSize.length, w = WallSize.wide } },
            };

        private readonly IRenderizable Game;
        private readonly BoardComp BoardComp;

        public WallRenderer(UxContext uxContext, IRenderizable game, IntPtr boardTexture)
            : base(uxContext)
        {
            Game = game;
            BoardComp = game
                .Entities
                .GetWithComponent<BoardComp>()
                .Single()
                .GetComponent<BoardComp>();

            Texture = boardTexture;
        }

        protected override void Render(IEntity entity, float interpolation)
        {
            var wallComp = entity.GetComponent<IWallComp>();

            var wallTiles = wallComp.Tiles
                .Where(wallTile => IsWallVisible(wallTile.Position.x, wallTile.Position.y))
                .ToList();

            if (wallComp.Facing == WallFacingDirections.RIGHT)
            {
                var deltaX = -(BoardComp.TILE_SIZE / 2);
                RenderVerticalWall(wallTiles, WallFacingDirections.RIGHT, deltaX);
            }
            else if (wallComp.Facing == WallFacingDirections.LEFT)
            {
                var deltaX = BoardComp.TILE_SIZE - (BoardComp.TILE_SIZE / 4);
                RenderVerticalWall(wallTiles, WallFacingDirections.LEFT, deltaX);
            }
            else if (wallComp.Facing == WallFacingDirections.DOWN)
            {
                var deltaY = -(BoardComp.TILE_SIZE / 2);
                RenderHorizontalWall(wallTiles, WallFacingDirections.DOWN, deltaY);
            }
            else if (wallComp.Facing == WallFacingDirections.UP)
            {
                var deltaY = BoardComp.TILE_SIZE - (BoardComp.TILE_SIZE / 4);
                RenderHorizontalWall(wallTiles, WallFacingDirections.UP, deltaY);
            }
        }

        private void RenderHorizontalWall(
            List<WallTile> wallTiles,
            WallFacingDirections facing,
            int deltaY
        ) =>
            wallTiles.ForEach(wallTile =>
                RenderWall(
                    x: (BoardComp.TILE_SIZE * wallTile.Position.x) - (WallSize.length - BoardComp.TILE_SIZE) / 2,
                    y: (BoardComp.TILE_SIZE * wallTile.Position.y) + deltaY,
                    textureRect: WallTexutreRects[(facing, wallTile.InitialTip, wallTile.FinalTip)]
                ));

        private void RenderVerticalWall(
            List<WallTile> wallTiles,
            WallFacingDirections facing,
            int deltaX
        ) =>
            wallTiles.ForEach(wallTile =>
                RenderWall(
                    x: (BoardComp.TILE_SIZE * wallTile.Position.x) + deltaX,
                    y: (BoardComp.TILE_SIZE * wallTile.Position.y) - (WallSize.length - BoardComp.TILE_SIZE) / 2,
                    textureRect: WallTexutreRects[(facing, wallTile.InitialTip, wallTile.FinalTip)]
                ));

        private bool IsWallVisible(int tileX, int tileY)
        {
            // return true;
            var tileCoords = (tileX, tileY);
            var tileId = BoardComp.TileIdByCoords[tileCoords];

            return Game.Entities[tileId].GetComponent<TileComp>().Visible;
        }

        private void RenderWall(int x, int y, SDL_Rect textureRect)
        {
            var tRect = new SDL_Rect()
            {
                x = GetPositionComponent(x, UxContext.CenterX),
                y = GetPositionComponent(y, UxContext.CenterY),
                w = textureRect.w,
                h = textureRect.h
            };
            SDL_RenderCopy(UxContext.WRenderer, Texture, ref textureRect, ref tRect);
        }

        protected static int GetPositionComponent(double positionComponent, int windowCenter) => // TODO DRY
            (int)positionComponent + windowCenter;

        protected override void Dispose(bool disposing) => base.Dispose(disposing);
    }
}
