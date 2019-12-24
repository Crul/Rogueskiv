﻿using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Crosscutting;
using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using System.Drawing;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class FOVRenderer : BaseItemRenderer<FOVComp>
    {
        private const int MAX_BLACK_OPACITY_FOR_REVEALED_BY_MAP = 0xDD;
        private const int MAX_BLACK_OPACITY = 0xAA;
        private const int MAX_BLACK_OPACITY_FOR_VISIBLE = MAX_BLACK_OPACITY - 0x11;

        private readonly BoardComp BoardComp;
        private readonly PlayerComp PlayerComp;

        public FOVRenderer(UxContext uxContext, IRenderizable game)
            : base(uxContext)
        {
            BoardComp = game.Entities.GetSingleComponent<BoardComp>();
            PlayerComp = game.Entities.GetSingleComponent<PlayerComp>();
        }

        protected override void Render(IEntity entity, FOVComp fovComp, float interpolation)
        {
            for (var x = 0; x < BoardComp.BoardSize.Width; x++)
                for (var y = 0; y < BoardComp.BoardSize.Height; y++)
                    RenderTileFOV(new Point(x, y), fovComp.GetTileFOVInfo(x, y));

            SDL_SetRenderDrawColor(UxContext.WRenderer, 0, 0, 0, 0);
        }

        private void RenderTileFOV(Point point, TileFOVInfo tileFOVInfo)
        {
            if (!tileFOVInfo.Hidden && !tileFOVInfo.VisibleByPlayer)
                return;

            byte alpha;
            if (tileFOVInfo.RevealedByMap)
                alpha = MAX_BLACK_OPACITY_FOR_REVEALED_BY_MAP;
            else if (tileFOVInfo.Hidden)
                alpha = MAX_BLACK_OPACITY;
            else
                alpha = (byte)(
                    MAX_BLACK_OPACITY_FOR_VISIBLE
                    * (tileFOVInfo.DistanceFromPlayer / (BoardComp.TILE_SIZE * PlayerComp.VisualRange))
                );

            var screenPosition = GetScreenPosition(point.Multiply(BoardComp.TILE_SIZE));

            var tRect = new SDL_Rect()
            {
                x = screenPosition.X,
                y = screenPosition.Y,
                w = BoardComp.TILE_SIZE,
                h = BoardComp.TILE_SIZE
            };

            SDL_SetRenderDrawColor(UxContext.WRenderer, 0, 0, 0, alpha);
            SDL_RenderFillRect(UxContext.WRenderer, ref tRect);
        }
    }
}
