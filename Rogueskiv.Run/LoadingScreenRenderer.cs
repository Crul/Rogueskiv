﻿using SDL2;
using Seedwork.Crosscutting;
using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using System;
using static SDL2.SDL;

namespace Rogueskiv.Run
{
    class LoadingScreenRenderer : TextRenderer
    {
        private readonly string LoadingText = "Creating floor...";
        private readonly SDL_Color LoadingTextColor = new SDL_Color() { r = 0xDD, g = 0xDD, b = 0xDD };
        private const int LOADING_FONT_SIZE = 24;

        public LoadingScreenRenderer(UxContext uxContext, string fontPath)
            : base(uxContext, GetFont(fontPath)) { }

        public void Render()
        {
            SDL_RenderClear(UxContext.WRenderer);
            Render(LoadingText, LoadingTextColor, UxContext.ScreenSize.Divide(2).ToPoint(), TextAlign.CENTER);
            SDL_RenderPresent(UxContext.WRenderer);
        }

        private static IntPtr GetFont(string fontPath) =>
            SDL_ttf.TTF_OpenFont(fontPath, LOADING_FONT_SIZE);

        protected override void Dispose(bool cleanManagedResources)
        {
            base.Dispose(cleanManagedResources);
            if (cleanManagedResources)
                SDL_ttf.TTF_CloseFont(Font);
        }
    }
}
