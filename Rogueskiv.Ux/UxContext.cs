using System;

namespace Rogueskiv.Ux
{
    class UxContext
    {
        public const int Zoom = 1;
        public const int CenterX = 0;
        public const int CenterY = 0;
        public readonly IntPtr WRenderer;

        public UxContext(IntPtr wRenderer) => WRenderer = wRenderer;
    }
}
