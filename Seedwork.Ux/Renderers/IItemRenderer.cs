using Seedwork.Core.Entities;
using System;
using System.Collections.Generic;

namespace Seedwork.Ux.Renderers
{
    public interface IItemRenderer : IDisposable
    {
        void Render(List<IEntity> entities, float interpolation);
    }
}
