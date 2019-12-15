using SDL2;
using Seedwork.Engine;
using Seedwork.Ux.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Seedwork.Ux
{
    public class GameRenderer : IGameRenderer
    {
        private const int WIDTH = 800;
        private const int HEIGHT = 520;

        protected readonly IntPtr Window;
        protected readonly IntPtr WRenderer;

        protected readonly IDictionary<Type, IItemRenderer> Renderers;

        public GameRenderer(string windowTitle = "MyGame")
        {
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);

            Window = SDL.SDL_CreateWindow(
                windowTitle,
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                WIDTH, HEIGHT,
                0
            );

            WRenderer = SDL.SDL_CreateRenderer(Window, -1, 0);
            SDL.SDL_SetRenderDrawColor(WRenderer, 0, 0, 0, 0);
            SDL.SDL_SetRenderDrawBlendMode(WRenderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);

            Renderers = new Dictionary<Type, IItemRenderer>();
        }

        public void Render(float interpolation)
        {
            SDL.SDL_RenderClear(WRenderer);
            RenderGame(interpolation);
            SDL.SDL_RenderPresent(WRenderer);
        }

        protected virtual void RenderGame(float interpolation) { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool cleanManagedResources)
        {
            if (cleanManagedResources)
                Renderers.ToList().ForEach(renderer => renderer.Value.Dispose());

            SDL.SDL_DestroyRenderer(WRenderer);
            SDL.SDL_DestroyWindow(Window);
            SDL.SDL_Quit();
        }
    }
}
