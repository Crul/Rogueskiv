using Seedwork.Core.Components;
using System;
using System.Drawing;
using static SDL2.SDL;

namespace Seedwork.Ux.SpriteProviders
{
    public interface ISpriteProvider<T> : IDisposable
        where T : IComponent
    {
        IntPtr GetTexture(T comp);
        SDL_Rect GetTextureRect(T comp, Point screenPosition);
        SDL_Rect GetOutputRect(T comp, Point position);
    }
}
