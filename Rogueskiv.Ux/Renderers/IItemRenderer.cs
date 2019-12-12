using Rogueskiv.Core.Entities;
using System;
using System.Collections.Generic;

namespace Rogueskiv.Ux.Renderers
{
    public interface IItemRenderer : IDisposable
    {
        void Render(List<IEntity> entities);
    }
}
