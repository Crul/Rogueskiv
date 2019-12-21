using SDL2;
using System;

namespace Seedwork.Ux
{
    public class UxContext : IDisposable
    {
        public int ScreenWidth { get; }
        public int ScreenHeight { get; }
        public int CenterX { get; set; } = 0;
        public int CenterY { get; set; } = 0;
        public IntPtr Window { get; }
        public IntPtr WRenderer { get; }

        public UxContext(string windowTitle, int screenWidth, int screenHeight)
        {
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;

            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
            SDL_ttf.TTF_Init();

            Window = SDL.SDL_CreateWindow(
                windowTitle,
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                screenWidth, screenHeight,
                0
            );

            WRenderer = SDL.SDL_CreateRenderer(Window, -1, 0);
            SDL.SDL_SetRenderDrawColor(WRenderer, 0, 0, 0, 0);
            SDL.SDL_SetRenderDrawBlendMode(WRenderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool cleanManagedResources)
        {
            SDL.SDL_DestroyRenderer(WRenderer);
            SDL.SDL_DestroyWindow(Window);
            SDL.SDL_Quit();
        }
    }
}
