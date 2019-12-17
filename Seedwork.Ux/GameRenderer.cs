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
        private readonly IntPtr Window;
        private readonly IntPtr WRenderer;

        protected IDictionary<Type, IItemRenderer> Renderers { get; }

        public GameRenderer(UxContext uxContext)
        {
            Window = uxContext.Window;
            WRenderer = uxContext.WRenderer;
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
        }
    }
}
