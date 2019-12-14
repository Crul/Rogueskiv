using System;

namespace Seedwork.Engine
{
    public interface IGameRenderer : IDisposable
    {
        void Render(float interpolation);
    }
}
