using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.Components.Walls;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Crosscutting;
using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class FOVRenderer : BaseItemRenderer<FOVComp>
    {
        private const int MAX_BLACK_OPACITY = 0xAA;
        private const int MAX_BLACK_OPACITY_FOR_VISIBLE = MAX_BLACK_OPACITY - 0x11;
        private const int VISUAL_RANGE = 10; // TODO proper visual range

        private readonly IRenderizable Game;
        private readonly BoardComp BoardComp;

        public FOVRenderer(UxContext uxContext, IRenderizable game)
            : base(uxContext)
        {
            Game = game;

            BoardComp = game
                .Entities
                .GetWithComponent<BoardComp>()
                .Single()
                .GetComponent<BoardComp>();
        }

        protected override void Render(IEntity entity, float interpolation)
        {
            BoardComp
                .TilePositionsByTileId
                .Keys
                .Select(tileId => Game.Entities[tileId])
                .ToList()
                .ForEach(tileEntity => RenderTileFOV(tileEntity.GetComponent<TileComp>()));

            SDL_SetRenderDrawColor(UxContext.WRenderer, 0, 0, 0, 0);
        }

        private void RenderTileFOV(TileComp positionComp)
        {
            byte alpha;
            if (positionComp.Visible && !positionComp.VisibleByPlayer)
                alpha = MAX_BLACK_OPACITY;
            else
                alpha = (byte)(
                    MAX_BLACK_OPACITY_FOR_VISIBLE
                    * (positionComp.DistanceFromPlayer / (BoardComp.TILE_SIZE * VISUAL_RANGE))
                );

            var screenPosition = GetScreenPosition(positionComp.Position)
                .Substract(BoardComp.TILE_SIZE / 2, BoardComp.TILE_SIZE / 2);

            var tRect = new SDL_Rect()
            {
                x = screenPosition.X,
                y = screenPosition.Y,
                w = BoardComp.TILE_SIZE,
                h = BoardComp.TILE_SIZE
            };

            SDL_SetRenderDrawColor(UxContext.WRenderer, 0, 0, 0, alpha);

            WallAdjustments(tRect, positionComp)
                .ForEach(rect =>
                    SDL_RenderFillRect(UxContext.WRenderer, ref rect)
                );
        }

        private List<SDL_Rect> WallAdjustments(SDL_Rect tRect, PositionComp positionComp)
        {
            // TODO simplify FOV of walls

            var originalTRect = tRect;
            var tRects = new List<SDL_Rect>();
            var wallFacingDirections = GetWallFacingDirections(positionComp.TilePos);

            var hasWallFacingLeft = wallFacingDirections.Contains(WallFacingDirections.LEFT);
            var hasWallFacingRight = wallFacingDirections.Contains(WallFacingDirections.RIGHT);
            var hasWallFacingUp = wallFacingDirections.Contains(WallFacingDirections.UP);
            var hasWallFacingDown = wallFacingDirections.Contains(WallFacingDirections.DOWN);

            if (hasWallFacingLeft)
                tRect.w += BoardComp.TILE_SIZE / 2;

            if (hasWallFacingRight)
            {
                tRect.x -= BoardComp.TILE_SIZE / 2;
                tRect.w += BoardComp.TILE_SIZE / 2;
            }

            if (hasWallFacingUp)
                tRect.h += BoardComp.TILE_SIZE / 2;

            if (hasWallFacingDown)
            {
                tRect.y -= BoardComp.TILE_SIZE / 2;
                tRect.h += BoardComp.TILE_SIZE / 2;
            }

            if (hasWallFacingLeft && hasWallFacingUp)
            {
                /*xxxx|
                  xxxx|
                  xxxxL___
                  ---¬????
                     |????
                     |????  */

                var diagonalTile = positionComp.TilePos.Add(1, 1);
                if (BoardComp.WallsByTiles.ContainsKey(diagonalTile))
                {
                    var wallFacingDirectionsOfDiagonal = GetWallFacingDirections(diagonalTile);

                    if (
                        wallFacingDirectionsOfDiagonal.Contains(WallFacingDirections.DOWN)
                        && wallFacingDirectionsOfDiagonal.Contains(WallFacingDirections.RIGHT)
                    )
                    {
                        tRect.h -= BoardComp.TILE_SIZE / 2;

                        tRects.Add(new SDL_Rect()
                        {
                            x = originalTRect.x,
                            y = originalTRect.y + originalTRect.h,
                            w = BoardComp.TILE_SIZE / 2,
                            h = BoardComp.TILE_SIZE / 2
                        });
                    }
                }
            }

            if (hasWallFacingRight && hasWallFacingDown)
            {
                var diagonalTile = positionComp.TilePos.Add(-1, -1);
                if (BoardComp.WallsByTiles.ContainsKey(diagonalTile))
                {
                    var wallFacingDirectionsOfDiagonal = GetWallFacingDirections(diagonalTile);

                    if (
                        wallFacingDirectionsOfDiagonal.Contains(WallFacingDirections.UP)
                        && wallFacingDirectionsOfDiagonal.Contains(WallFacingDirections.LEFT)
                    )
                    {
                        tRect.y += BoardComp.TILE_SIZE / 2;
                        tRect.h -= BoardComp.TILE_SIZE / 2;

                        tRects.Add(new SDL_Rect()
                        {
                            x = originalTRect.x + BoardComp.TILE_SIZE / 2,
                            y = originalTRect.y - BoardComp.TILE_SIZE / 2,
                            w = BoardComp.TILE_SIZE / 2,
                            h = BoardComp.TILE_SIZE / 2
                        });
                    }
                }
            }

            if (hasWallFacingRight && hasWallFacingUp)
            {
                /*   |xxxx
                     |xxxx
                  ___Jxxxx
                  ???? ---
                  ????|
                  ????|    */

                var diagonalTile = positionComp.TilePos.Add(-1, 1);
                if (BoardComp.WallsByTiles.ContainsKey(diagonalTile))
                {
                    var wallFacingDirectionsOfDiagonal = GetWallFacingDirections(diagonalTile);

                    if (
                        wallFacingDirectionsOfDiagonal.Contains(WallFacingDirections.DOWN)
                        && wallFacingDirectionsOfDiagonal.Contains(WallFacingDirections.LEFT)
                    )
                    {
                        tRect.h -= BoardComp.TILE_SIZE / 2;

                        tRects.Add(new SDL_Rect()
                        {
                            x = originalTRect.x + BoardComp.TILE_SIZE / 2,
                            y = originalTRect.y + originalTRect.h,
                            w = BoardComp.TILE_SIZE / 2,
                            h = BoardComp.TILE_SIZE / 2
                        });
                    }
                }
            }

            if (hasWallFacingLeft && hasWallFacingDown)
            {
                var diagonalTile = positionComp.TilePos.Add(1, -1);
                if (BoardComp.WallsByTiles.ContainsKey(diagonalTile))
                {
                    var wallFacingDirectionsOfDiagonal = GetWallFacingDirections(diagonalTile);

                    if (
                        wallFacingDirectionsOfDiagonal.Contains(WallFacingDirections.UP)
                        && wallFacingDirectionsOfDiagonal.Contains(WallFacingDirections.RIGHT)
                    )
                    {
                        tRect.y += BoardComp.TILE_SIZE / 2;
                        tRect.h -= BoardComp.TILE_SIZE / 2;

                        tRects.Add(new SDL_Rect()
                        {
                            x = originalTRect.x,
                            y = originalTRect.y - BoardComp.TILE_SIZE / 2,
                            w = BoardComp.TILE_SIZE / 2,
                            h = BoardComp.TILE_SIZE / 2
                        });
                    }
                }
            }

            tRects.Add(tRect);

            return tRects;
        }

        private List<WallFacingDirections> GetWallFacingDirections(Point tilePos) =>
             BoardComp
                .WallsByTiles[tilePos]
                .Select(wallId => Game.Entities[wallId].GetComponent<IWallComp>().Facing)
                .ToList();
    }
}
