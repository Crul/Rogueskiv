using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Walls;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Crosscutting;
using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            > WallTextureRects = new Dictionary<
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
            BoardComp = game.Entities.GetSingleComponent<BoardComp>();
            Texture = boardTexture;
        }

        protected override void Render(IEntity entity, float interpolation)
        {
            var wallComp = entity.GetComponent<IWallComp>();

            var wallTiles = wallComp.Tiles
                .Where(wallTile => IsWallVisible(wallTile.TilePos))
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
            {
                var wallPosition = wallTile
                    .TilePos
                    .Multiply(BoardComp.TILE_SIZE)
                    .Add(
                        -(WallSize.length - BoardComp.TILE_SIZE) / 2,
                        deltaY
                    );

                RenderWall(
                    wallPosition,
                    textureRect: WallTextureRects[(facing, wallTile.InitialTip, wallTile.FinalTip)]
                );
            });

        private void RenderVerticalWall(
            List<WallTile> wallTiles,
            WallFacingDirections facing,
            int deltaX
        ) =>
            wallTiles.ForEach(wallTile =>
            {
                var wallPosition = wallTile
                    .TilePos
                    .Multiply(BoardComp.TILE_SIZE)
                    .Add(
                        deltaX,
                        -(WallSize.length - BoardComp.TILE_SIZE) / 2
                    );

                RenderWall(
                    wallPosition,
                    textureRect: WallTextureRects[(facing, wallTile.InitialTip, wallTile.FinalTip)]
                );
            });

        private bool IsWallVisible(Point tilePos)
        {
            var tileId = BoardComp.TileIdByTilePos[tilePos];

            return Game.Entities[tileId].GetComponent<TileComp>().Visible;
        }

        private void RenderWall(PointF position, SDL_Rect textureRect)
        {
            var screenPosition = GetScreenPosition(position);
            var tRect = new SDL_Rect()
            {
                x = screenPosition.X,
                y = screenPosition.Y,
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
