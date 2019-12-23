using SDL2;
using Seedwork.Crosscutting;
using System;
using System.Drawing;

namespace Seedwork.Ux
{
    public class UxContext : IDisposable
    {
        public Size ScreenSize { get; private set; }
        public Point Center { get; set; }
        public IntPtr Window { get; }
        public IntPtr WRenderer { get; }

        public UxContext(string windowTitle, Size? screenSize = null)
        {
            var sdlWindowFlags = SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE;
            if (screenSize.HasValue)
            {
                ScreenSize = screenSize.Value;
                Center = ScreenSize.Divide(2).ToPoint();
            }
            else
                sdlWindowFlags |= SDL.SDL_WindowFlags.SDL_WINDOW_MAXIMIZED;

            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
            SDL_ttf.TTF_Init();

            Window = SDL.SDL_CreateWindow(
                windowTitle,
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                ScreenSize.Width, ScreenSize.Height,
                sdlWindowFlags
            );

            WRenderer = SDL.SDL_CreateRenderer(Window, -1, 0);
            SDL.SDL_SetRenderDrawColor(WRenderer, 0, 0, 0, 0);
            SDL.SDL_SetRenderDrawBlendMode(WRenderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
            // SDL.SDL_RenderSetLogicalSize(WRenderer, screenSize.Width, screenSize.Height);
        }

        public void OnWindowResize(int width, int height)
        {
            ScreenSize = new Size(width, height);
            Center = ScreenSize.Divide(2).ToPoint();
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
