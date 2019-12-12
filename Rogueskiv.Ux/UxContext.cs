using System;

namespace Rogueskiv.Ux
{
    class UxContext
    {
        public const int Zoom = 1;
        public const int CenterX = 300;
        public const int CenterY = 300;
        public readonly IntPtr WRenderer;

        public UxContext(IntPtr wRenderer) => WRenderer = wRenderer;
    }
}
