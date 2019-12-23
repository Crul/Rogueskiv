﻿using Seedwork.Core.Components;
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
            entities.ForEach(e => Render(e, e.GetComponent<T>(), interpolation));

        protected abstract void Render(IEntity entity, T comp, float interpolation);

        protected Point GetScreenPosition(PointF position) =>
            position.Add(UxContext.Center).ToPoint();

        protected virtual void Dispose(bool cleanManagedResources) { }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
