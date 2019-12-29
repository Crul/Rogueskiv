using Seedwork.Core.Components;
using System;
using System.Drawing;
using static SDL2.SDL;

namespace Seedwork.Ux.SpriteProviders
{
    public abstract class SpriteProvider<T> : ISpriteProvider<T>
        where T : IComponent
    {
        public abstract IntPtr GetTexture(T comp);
        public abstract SDL_Rect GetTextureRect(T comp, Point screenPosition);
        public abstract SDL_Rect GetOutputRect(T comp, Point screenPosition);

        protected SDL_Rect GetOutputRect(Point screenPosition, Size outputSize) =>
            new SDL_Rect()
            {
                x = screenPosition.X - (outputSize.Width / 2),
                y = screenPosition.Y - (outputSize.Height / 2),
                w = outputSize.Width,
                h = outputSize.Height
            };

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool cleanManagedResources) { }
    }
}
