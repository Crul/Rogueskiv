﻿using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Walls;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Crosscutting;
using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using System;
using System.Drawing;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class BoardRenderer : IRenderer
    {
        private readonly UxContext UxContext;
        private IntPtr BoardBufferTexture;
        private FOVComp FOVComp;
        private Size BoardSize;

        public BoardRenderer(UxContext uxContext, IRenderizable game, IntPtr boardTexture)
        {
            UxContext = uxContext;
            CreateBuffer(game, boardTexture);
        }

        public void Render() =>
            FOVComp.ForAllSubTiles(tileFOVInfo =>
            {
                if (tileFOVInfo.Visible)
                    Render(tileFOVInfo);
            });

        private void Render(TileFOVInfo tileFOVInfo)
        {
            var textureRect = new SDL_Rect()
            {
                x = (int)tileFOVInfo.Position.X,
                y = (int)tileFOVInfo.Position.Y,
                w = BoardComp.TILE_SIZE / 2,
                h = BoardComp.TILE_SIZE / 2
            };

            var screenPos = tileFOVInfo.Position.Add(UxContext.Center).ToPoint();
            var outputRect = new SDL_Rect()
            {
                x = screenPos.X,
                y = screenPos.Y,
                w = textureRect.w,
                h = textureRect.h
            };

            SDL_RenderCopy(UxContext.WRenderer, BoardBufferTexture, ref textureRect, ref outputRect);
        }

        public void RecreateBuffer(IRenderizable game, IntPtr boardTexture)
        {
            SDL_DestroyTexture(BoardBufferTexture);
            CreateBuffer(game, boardTexture);
        }

        private void CreateBuffer(IRenderizable game, IntPtr boardTexture)
        {
            FOVComp = game.Entities.GetSingleComponent<FOVComp>();

            var boardComp = game.Entities.GetSingleComponent<BoardComp>();
            BoardSize = boardComp.BoardSize.Multiply(BoardComp.TILE_SIZE);

            BoardBufferTexture = SDL_CreateTexture(
                UxContext.WRenderer,
                SDL_PIXELFORMAT_RGBA8888,
                (int)SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET,
                BoardSize.Width,
                BoardSize.Height
            );
            SDL_SetTextureBlendMode(BoardBufferTexture, SDL_BlendMode.SDL_BLENDMODE_BLEND);
            SDL_SetRenderTarget(UxContext.WRenderer, BoardBufferTexture);

            using var tileRenderer = new TileRenderer(UxContext, game);
            var tileEntities = game.Entities.GetWithComponent<TileComp>();
            tileRenderer.Render(tileEntities, 0);

            using var wallRenderer = new WallRenderer(UxContext, game, boardTexture);
            var wallEntities = game.Entities.GetWithComponent<IWallComp>();
            wallRenderer.Render(wallEntities, 0);

            using var downStairsRenderer = new DownStairsRenderer(UxContext, game, boardTexture);
            var downStairEntities = game.Entities.GetWithComponent<DownStairsComp>();
            downStairsRenderer.Render(downStairEntities, 0);

            using var upStairsRenderer = new UpStairsRenderer(UxContext, game, boardTexture);
            var upStairsEntities = game.Entities.GetWithComponent<UpStairsComp>();
            upStairsRenderer.Render(upStairsEntities, 0);

            SDL_SetRenderTarget(UxContext.WRenderer, IntPtr.Zero);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool cleanManagedResources)
        {
            if (cleanManagedResources)
                SDL_DestroyTexture(BoardBufferTexture);
        }
    }
}
