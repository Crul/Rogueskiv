using System;

namespace Seedwork.Engine
{
    public interface IGameRenderer : IDisposable
    {
        void Render(float interpolation);
        void Stop();
        void Restart();
        void AddRenderOnEnd(Action renderOnEnd);
        void RecreateBufferTextures();
    }
}
