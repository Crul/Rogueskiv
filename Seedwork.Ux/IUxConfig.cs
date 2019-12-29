using System.Drawing;

namespace Seedwork.Ux
{
    public interface IUxConfig
    {
        public Size ScreenSize { get; }
        public bool Maximized { get; }
    }
}
