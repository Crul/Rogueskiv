using SDL2;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Engine;
using Seedwork.Ux.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Seedwork.Ux
{
    public class GameRenderer : IGameRenderer
    {
        protected readonly IRenderizable Game;
        private readonly List<Action> RenderOnEndActions;

        protected IntPtr WRenderer { get; }
        protected List<IRenderer> Renderers { get; }
        protected IDictionary<Type, ICompRenderer> CompRenderers { get; }

        public GameRenderer(UxContext uxContext, IRenderizable game)
        {
            Game = game;
            WRenderer = uxContext.WRenderer;
            RenderOnEndActions = new List<Action>();
            Renderers = new List<IRenderer>();
            CompRenderers = new Dictionary<Type, ICompRenderer>();
        }

        public virtual void Stop() => DisposeBufferTextures();

        public virtual void Restart() => RecreateBufferTextures();

        public void Render(float interpolation)
        {
            RenderGame(interpolation);
            SDL.SDL_RenderPresent(WRenderer);
        }

        protected virtual void RenderGame(float interpolation)
        {
            RenderOnEndActions.Clear();
            Renderers.ForEach(r => r.Render());
            CompRenderers.ToList()
                .ForEach(r =>
                    r.Value.Render(Game.Entities.GetWithComponent(r.Key), interpolation)
                );
            RenderOnEndActions.ForEach(renderOnEnd => renderOnEnd());
        }

        public void AddRenderOnEnd(Action renderOnEnd) =>
            RenderOnEndActions.Add(renderOnEnd);

        public virtual void RecreateBufferTextures() { }

        protected virtual void DisposeBufferTextures() { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool cleanManagedResources)
        {
            if (cleanManagedResources)
            {
                Renderers.ForEach(renderer => renderer.Dispose());
                CompRenderers.ToList().ForEach(renderer => renderer.Value.Dispose());
            }
        }
    }
}
