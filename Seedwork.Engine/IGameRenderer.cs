using System;

namespace Seedwork.Engine
{
    public interface IGameRenderer : IDisposable
    {
        void Render(float interpolation);
        void Reset();
        void AddRenderOnEnd(Action renderOnEnd);
        void RecreateTextures();
    }
}
