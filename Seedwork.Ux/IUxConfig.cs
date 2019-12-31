using System.Drawing;

namespace Seedwork.Ux
{
    public interface IUxConfig
    {
        public Size ScreenSize { get; }
        public bool Maximized { get; }
        public bool SoundsOn { get; set; }
    }
}
