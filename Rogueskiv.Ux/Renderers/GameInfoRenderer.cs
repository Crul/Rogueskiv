using Rogueskiv.Core.Components;
using Seedwork.Core.Entities;
using Seedwork.Crosscutting;
using Seedwork.Engine;
using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using System;
using System.Drawing;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class GameInfoRenderer : TextCompRenderer<TimerComp>
    {
        private const int HEIGHT = 24;
        private const int MARGIN = 25;
        private readonly SDL_Color TEXT_COLOR = new SDL_Color() { r = 0xFF, g = 0xFF, b = 0xFF, a = 0xFF };

        private readonly int Floor;
        private readonly long GameTicks;
        private readonly bool InGameTimeVisible;
        private readonly bool RealTimeVisible;

        public GameInfoRenderer(
            UxContext uxContext,
            IGameContext gameContext,
            IntPtr font,
            int floor,
            bool inGameTimeVisible,
            bool realTimeVisible
        ) : base(uxContext, font)
        {
            Floor = floor;
            GameTicks = gameContext.GameTicks;
            InGameTimeVisible = inGameTimeVisible;
            RealTimeVisible = realTimeVisible;
        }

        protected override void Render(IEntity entity, TimerComp timerComp, float interpolation)
        {
            var floorText = $"FLOOR {Floor}";
            var position = new Point(MARGIN, MARGIN);
            TextRenderer.Render(floorText, TEXT_COLOR, position, TextAlign.TOP_LEFT);

            RenderTimers(timerComp);
        }

        private void RenderTimers(TimerComp timerComp)
        {
            var realTime = timerComp.GetRealTime();
            var format = GetTimeFormat(realTime);
            var position = new Point(x: UxContext.ScreenSize.Width - MARGIN, y: MARGIN);

            if (InGameTimeVisible)
            {
                var inGameTicks = timerComp.InGameTime * GameTicks;
                var igtText = new TimeSpan(inGameTicks).ToString(format);
                if (RealTimeVisible)
                    igtText = $"IGT: {igtText}";

                TextRenderer.Render(igtText, TEXT_COLOR, position, TextAlign.TOP_RIGHT);
                position = position.Add(y: HEIGHT);
            }

            if (RealTimeVisible)
            {
                var rtaText = realTime.ToString(format);
                if (InGameTimeVisible)
                    rtaText = $"RTA: {rtaText}";

                TextRenderer.Render(rtaText, TEXT_COLOR, position, TextAlign.TOP_RIGHT);
            }
        }

        private static string GetTimeFormat(TimeSpan time)
        {
            var format = "ss'.'ff";
            if (time.Minutes > 0)
                format = $"mm':'{format}";

            if (time.Hours > 0)
                format = $"hh':'{format}";

            if (time.Days > 0)
                format = $"d' day{GetPlural(time.Days)} '{format}";

            return format;
        }

        private static string GetPlural(int value) => value > 0 ? "s" : string.Empty;

        protected override SDL_Color GetColor(TimerComp component) =>
            throw new NotImplementedException();

        protected override Point GetPosition(TimerComp component) =>
            throw new NotImplementedException();

        protected override string GetText(TimerComp component) =>
            throw new NotImplementedException();
    }
}
