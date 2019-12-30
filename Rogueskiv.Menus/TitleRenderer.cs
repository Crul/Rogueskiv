using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using System;
using System.Drawing;
using static SDL2.SDL;

namespace Rogueskiv.Menus
{
    class TitleRenderer : IRenderer
    {
        private const int MARGIN_X = 240;
        private const int MARGIN_Y = 60;

        private readonly SDL_Color TitleColor =
            new SDL_Color() { r = 0x7C, g = 0xD4, b = 0xDA, a = 0xFF };

        protected readonly UxContext UxContext;
        protected readonly TextRenderer TextRenderer;

        public TitleRenderer(UxContext uxContext, IntPtr font)
        {
            UxContext = uxContext;
            TextRenderer = new TextRenderer(uxContext, font);
        }

        public void Render()
        {
            var titlePosition = new Point(
                x: UxContext.ScreenSize.Width - MARGIN_X,
                y: MARGIN_Y
            );
            TextRenderer.Render(UxContext.Title, TitleColor, titlePosition);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool cleanManagedResources)
        {
            if (cleanManagedResources)
                TextRenderer.Dispose();
        }
    }
}
