using Seedwork.Crosscutting;
using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using static SDL2.SDL;

namespace Rogueskiv.Menus.Renderers
{
    class TitleRenderer : IRenderer
    {
        private const int MARGIN_X = 60;
        private const int MARGIN_Y = 30;
        private const int TITLE_HEIGHT = 48;

        private readonly Point MinPosition = new Point(30, MARGIN_Y);

        private readonly SDL_Color TitleColor =
            new SDL_Color() { r = 0x7C, g = 0xD4, b = 0xDA, a = 0xFF };

        private readonly SDL_Color VersionColor =
            new SDL_Color() { r = 0x7C, g = 0xD4, b = 0xDA, a = 0x88 };

        private readonly UxContext UxContext;
        private readonly TextRenderer TextRenderer;
        private readonly TextRenderer VersionRenderer;
        private readonly string Version;

        public TitleRenderer(UxContext uxContext, IntPtr titleFont, IntPtr versionFont)
        {
            UxContext = uxContext;
            TextRenderer = new TextRenderer(uxContext, titleFont);
            VersionRenderer = new TextRenderer(uxContext, versionFont);

            var assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            Version = fileVersionInfo.ProductVersion;
        }

        public void Render()
        {
            var titlePosition = new Point(
                x: UxContext.ScreenSize.Width - MARGIN_X,
                y: MARGIN_Y
            );
            TextRenderer.Render(
                UxContext.Title,
                TitleColor,
                titlePosition,
                TextAlign.TOP_RIGHT,
                minPosition: MinPosition
            );
            var versionPosition = titlePosition.Add(x: -10, y: TITLE_HEIGHT);
            VersionRenderer.Render(
                $"v{Version}",
                VersionColor,
                versionPosition,
                TextAlign.TOP_RIGHT
            );
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool cleanManagedResources)
        {
            if (cleanManagedResources)
            {
                TextRenderer.Dispose();
                VersionRenderer.Dispose();
            }
        }
    }
}
