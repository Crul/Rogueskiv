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
        private readonly IntPtr WRenderer;
        private readonly IRenderizable Game;
        private readonly List<Action> RenderOnEndActions;

        protected IDictionary<Type, IItemRenderer> Renderers { get; }

        public GameRenderer(UxContext uxContext, IRenderizable game)
        {
            Game = game;
            WRenderer = uxContext.WRenderer;
            RenderOnEndActions = new List<Action>();
            Renderers = new Dictionary<Type, IItemRenderer>();
        }

        public virtual void Reset() { }

        public void Render(float interpolation)
        {
            SDL.SDL_RenderClear(WRenderer);
            RenderGame(interpolation);
            SDL.SDL_RenderPresent(WRenderer);
        }

        protected virtual void RenderGame(float interpolation)
        {
            RenderOnEndActions.Clear();
            Renderers.ToList()
                .ForEach(r =>
                    r.Value.Render(Game.Entities.GetWithComponent(r.Key), interpolation)
                );
            RenderOnEndActions.ForEach(renderOnEnd => renderOnEnd());
        }

        public void AddRenderOnEnd(Action renderOnEnd) =>
            RenderOnEndActions.Add(renderOnEnd);

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
