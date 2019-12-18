using Seedwork.Core.Components;
using Seedwork.Core.Entities;
using System;
using System.Collections.Generic;

namespace Seedwork.Ux.Renderers
{
    public abstract class BaseItemRenderer<T> : IItemRenderer
        where T : IComponent
    {
        protected readonly UxContext UxContext;

        public BaseItemRenderer(UxContext uxContext) => UxContext = uxContext;

        public void Render(List<IEntity> entities, float interpolation) =>
            entities.ForEach(e => RenderIfComponent(e, interpolation));

        private void RenderIfComponent(IEntity entity, float interpolation)
        {
            if (entity.HasComponent<T>())
                Render(entity, interpolation);
        }

        protected abstract void Render(IEntity entity, float interpolation);

        protected virtual void Dispose(bool disposing) { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
