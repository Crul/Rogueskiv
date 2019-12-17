using System;

namespace Seedwork.Ux
{
    public class UxContext
    {
        public const int Zoom = 1;
        public int CenterX = 0;
        public int CenterY = 0;
        public readonly IntPtr WRenderer;

        public const int SCREEN_WIDTH = 800;
        public const int SCREEN_HEIGHT = 520;


        public UxContext(IntPtr wRenderer) => WRenderer = wRenderer;
    }
}
