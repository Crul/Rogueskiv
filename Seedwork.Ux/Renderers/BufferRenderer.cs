﻿using System;
using static SDL2.SDL;

namespace Seedwork.Ux.Renderers
{
    public class BufferRenderer
    {
        private readonly UxContext UxContext;
        private IntPtr BufferTexture;

        public BufferRenderer(UxContext uxContext)
        {
            UxContext = uxContext;
            SetBufferTexture();
        }

        private void SetBufferTexture()
        {
            BufferTexture = SDL_CreateTexture(
                UxContext.WRenderer,
                SDL_PIXELFORMAT_RGBA8888,
                (int)SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET,
                UxContext.ScreenSize.Width,
                UxContext.ScreenSize.Height
            );
            SDL_SetTextureBlendMode(BufferTexture, SDL_BlendMode.SDL_BLENDMODE_BLEND);
        }

        public void ResetBuffer()
        {
            SDL_SetRenderTarget(UxContext.WRenderer, BufferTexture);
            SDL_RenderClear(UxContext.WRenderer);
        }

        public void Render()
        {
            SDL_SetRenderTarget(UxContext.WRenderer, IntPtr.Zero);
            SDL_RenderCopy(UxContext.WRenderer, BufferTexture, IntPtr.Zero, IntPtr.Zero);
        }

        public void RecreateBuffer()
        {
            Dispose(true);
            SetBufferTexture();
        }

        protected virtual void Dispose(bool cleanManagedResources)
        {
            if (cleanManagedResources)
                SDL_DestroyTexture(BufferTexture);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
