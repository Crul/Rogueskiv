using System;

namespace Rogueskiv.Engine
{
    public interface IRenderer : IDisposable
    {
        void Render(float interpolation);
    }
}
