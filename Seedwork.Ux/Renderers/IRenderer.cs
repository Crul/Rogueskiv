using System;

namespace Seedwork.Ux.Renderers
{
    public interface IRenderer : IDisposable
    {
        void Render();
    }
}
