using Seedwork.Core.Components;
using Seedwork.Core.Entities;
using Seedwork.Crosscutting;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Seedwork.Ux.Renderers
{
    public abstract class BaseItemRenderer<T> : IItemRenderer
        where T : IComponent
    {
        protected readonly UxContext UxContext;

        protected BaseItemRenderer(UxContext uxContext) => UxContext = uxContext;

        public void Render(List<IEntity> entities, float interpolation) =>
            entities.ForEach(e => RenderIfComponent(e, interpolation));

        private void RenderIfComponent(IEntity entity, float interpolation)
        {
            if (entity.HasComponent<T>())
                Render(entity, interpolation);
        }

        protected abstract void Render(IEntity entity, float interpolation);

        protected Point GetScreenPosition(PointF position) =>
            position.Add(UxContext.Center).ToPoint();

        protected virtual void Dispose(bool disposing) { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
