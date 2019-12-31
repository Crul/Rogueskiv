using SDL2;
using Seedwork.Crosscutting;
using System;
using System.Drawing;

namespace Seedwork.Ux
{
    public class UxContext : IDisposable
    {
        public string Title { get; }
        public Size ScreenSize { get; private set; }
        public Point Center { get; set; }
        public IntPtr Window { get; }
        public IntPtr WRenderer { get; }

        public UxContext(string windowTitle, IUxConfig uxConfig)
        {
            Title = windowTitle;

            if (uxConfig != null)
                OnWindowResize(uxConfig.ScreenSize);

            var sdlWindowFlags = SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE;
            if (uxConfig?.Maximized ?? true)
                sdlWindowFlags |= SDL.SDL_WindowFlags.SDL_WINDOW_MAXIMIZED;

            SDL.SDL_Init(SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_AUDIO);
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

            SDL_mixer.Mix_OpenAudio(44100, SDL_mixer.MIX_DEFAULT_FORMAT, 2, 2048);
        }

        public void OnWindowResize(int width, int height) =>
            OnWindowResize(new Size(width, height));

        private void OnWindowResize(Size screenSize)
        {
            Center = Center.Add(screenSize.Substract(ScreenSize).Divide(2).ToPoint());
            ScreenSize = screenSize;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool cleanManagedResources)
        {
            SDL_mixer.Mix_Quit();
            SDL.SDL_DestroyRenderer(WRenderer);
            SDL.SDL_DestroyWindow(Window);
            SDL.SDL_Quit();
        }
    }
}
