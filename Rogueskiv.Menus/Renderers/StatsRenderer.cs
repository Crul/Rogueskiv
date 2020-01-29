using Seedwork.Core.Entities;
using Seedwork.Crosscutting;
using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static SDL2.SDL;

namespace Rogueskiv.Menus.Renderers
{
    class StatsRenderer : TextCompRenderer<StatsComp>
    {
        private const int PADDING = 10;
        private const byte BGR_OPACITY = 0xF0;
        private const int LINE_HEIGHT = 24;
        private const int TOP_MARGIN_Y = 120;
        private const int TOTAL_MARGIN_Y = TOP_MARGIN_Y + 180;
        private const string COLUMN_PADDING_CHARS = "  ";

        private readonly List<string> StatTitles =
            new List<string>
            {
                "result", "started on", "game mode", "floors", "died on", "health", "RTA", "IGT"
            };

        private readonly List<int> ColumnWidths =
            new List<int>
            {
                6, 12, 10, 6, 7, 6, 10, 10
            };

        public StatsRenderer(UxContext uxContext, IntPtr font) : base(uxContext, font) { }

        protected override void Render(IEntity entity, StatsComp statsComp, float interpolation)
        {
            if (!statsComp.Visible)
                return;

            var pageSize = (UxContext.ScreenSize.Height - TOTAL_MARGIN_Y) / LINE_HEIGHT;
            var pageCount = (int)Math.Ceiling((double)statsComp.GameStats.Count / pageSize);
            statsComp.Page = Math.Max(0, Math.Min(pageCount - 1, statsComp.Page));

            var textLines = statsComp
                .GameStats
                .Select(JoinColumns)
                .Skip(statsComp.Page * pageSize)
                .Take(pageSize)
                .ToList();

            var position = GetPosition(statsComp);
            var textColor = GetColor(statsComp);
            var aligment = GetAligment();

            if (textLines.Any())
            {
                textLines.AddRange(Enumerable.Range(0, 1 + pageSize - textLines.Count).Select(_ => string.Empty));
                textLines.Add($"Page {statsComp.Page + 1} / {pageCount}");

                TextRenderer.Render(
                    JoinColumns(StatTitles),
                    textColor,
                    position,
                    aligment,
                    position => RenderBgr(position, textLines.Count + 3)
                );
            }
            else
                textLines.Add("No games played");

            textLines.ToList().ForEach(textLine =>
            {
                position = position.Add(y: LINE_HEIGHT);
                if (!string.IsNullOrEmpty(textLine))
                    TextRenderer.Render(textLine, textColor, position, aligment);
            });
        }

        private string JoinColumns(List<string> values)
        {
            var formattedValues = values
                .Zip(ColumnWidths)
                .Select(FormatColumn)
                .ToList();

            return string.Join(COLUMN_PADDING_CHARS, formattedValues);
        }

        private string FormatColumn((string text, int width) data)
            => data.text.Substring(0, Math.Min(data.text.Length, data.width)).PadRight(data.width);

        protected override SDL_Color GetColor(StatsComp component) =>
            new SDL_Color() { r = 0xFF, g = 0xFF, b = 0xFF, a = 0xFF };

        protected override Point GetPosition(StatsComp component) =>
            new Point(UxContext.ScreenSize.Width / 2, TOP_MARGIN_Y);

        protected override string GetText(StatsComp component) => string.Empty;

        private void RenderBgr(Point position, int textLines)
        {
            var bgr = new SDL_Rect()
            {
                x = PADDING,
                y = position.Y - PADDING,
                w = UxContext.ScreenSize.Width - 2 * PADDING,
                h = TextRenderer.SurfaceCache.h * textLines + 2 * PADDING
            };
            SDL_GetRenderDrawColor(UxContext.WRenderer, out byte r, out byte g, out byte b, out byte a);
            SDL_SetRenderDrawColor(UxContext.WRenderer, 0x00, 0x00, 0x00, BGR_OPACITY);
            SDL_RenderFillRect(UxContext.WRenderer, ref bgr);
            SDL_SetRenderDrawColor(UxContext.WRenderer, r, g, b, a);
        }
    }
}
