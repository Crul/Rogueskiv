﻿using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Walls;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Crosscutting;
using Seedwork.Ux;
using System;
using System.IO;
using System.Linq;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class FOVRenderer : PositionRenderer<IFOVComp>
    {
        private const int MAX_BLACK_OPACITY = 0xAA;
        private const int MAX_BLACK_OPACITY_FOR_VISIBLE = MAX_BLACK_OPACITY - 0x11;
        private const int VISUAL_RANGE = 10; // TODO proper visual range

        private readonly IRenderizable Game;
        private readonly BoardComp BoardComp;

        public FOVRenderer(UxContext uxContext, IRenderizable game)
            : base(
                  uxContext,
                  Path.Combine("imgs", "tile.png"),  // TODO not needed
                  new SDL_Rect { x = 0, y = 0, w = 48, h = 48 },
                  new Tuple<int, int>(BoardComp.TILE_SIZE, BoardComp.TILE_SIZE)
            )
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
            // TODO DRY PositionRenderer / ItemRender
            var positionComp = entity.GetComponent<TileComp>();

            byte alpha;
            if (positionComp.Visible && !positionComp.VisibleByPlayer)
                alpha = MAX_BLACK_OPACITY;
            else
                alpha = (byte)(
                    MAX_BLACK_OPACITY_FOR_VISIBLE
                    * (positionComp.DistanceFromPlayer / (BoardComp.TILE_SIZE * VISUAL_RANGE))
                );

            var position = GetXY(entity, positionComp, interpolation);

            var wallFacingDirections = BoardComp
                .WallsByTiles[position.Divide(BoardComp.TILE_SIZE)]
                .Select(wallId => Game.Entities[wallId].GetComponent<IWallComp>().Facing);

            var screenPosition = GetScreenPosition(position)
                .Substract(OutputSize.Item1 / 2, OutputSize.Item2 / 2);

            var tRect = new SDL_Rect()
            {
                x = screenPosition.X,
                y = screenPosition.Y,
                w = OutputSize.Item1,
                h = OutputSize.Item2
            };

            if (wallFacingDirections.Contains(WallFacingDirections.LEFT))
                tRect.w += BoardComp.TILE_SIZE / 2;

            if (wallFacingDirections.Contains(WallFacingDirections.RIGHT))
            {
                tRect.x -= BoardComp.TILE_SIZE / 2;
                tRect.w += BoardComp.TILE_SIZE / 2;
            }

            if (wallFacingDirections.Contains(WallFacingDirections.UP))
                tRect.h += BoardComp.TILE_SIZE / 2;

            if (wallFacingDirections.Contains(WallFacingDirections.DOWN))
            {
                tRect.y -= BoardComp.TILE_SIZE / 2;
                tRect.h += BoardComp.TILE_SIZE / 2;
            }

            SDL_SetRenderDrawColor(UxContext.WRenderer, 0, 0, 0, alpha);
            SDL_RenderFillRect(UxContext.WRenderer, ref tRect);
            SDL_SetRenderDrawColor(UxContext.WRenderer, 0, 0, 0, 0);
        }
    }
}
