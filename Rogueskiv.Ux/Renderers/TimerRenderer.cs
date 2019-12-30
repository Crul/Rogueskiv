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
    class TimerRenderer : TextRenderer<TimerComp>
    {
        private const int WIDTH = 120;
        private const int HEIGHT = 24;
        private const int MARGIN = 25;
        private readonly SDL_Color TEXT_COLOR = new SDL_Color() { r = 0xFF, g = 0xFF, b = 0xFF, a = 0xFF };
        private readonly long GameTicks;
        private readonly bool InGameTimeVisible;
        private readonly bool RealTimeVisible;

        public TimerRenderer(
            UxContext uxContext,
            IGameContext gameContext,
            IntPtr font,
            bool inGameTimeVisible,
            bool realTimeVisible
        ) : base(uxContext, font)
        {
            GameTicks = gameContext.GameTicks;
            InGameTimeVisible = inGameTimeVisible;
            RealTimeVisible = realTimeVisible;
        }

        protected override void Render(IEntity entity, TimerComp timerComp, float interpolation)
        {
            var realTime = timerComp.GetRealTime();
            var format = GetTimeFormat(realTime);
            var position = new Point(x: UxContext.ScreenSize.Width - (MARGIN + WIDTH), y: MARGIN);

            if (InGameTimeVisible)
            {
                var inGameTicks = timerComp.InGameTime * GameTicks;
                var igtText = new TimeSpan(inGameTicks).ToString(format);
                if (RealTimeVisible)
                    igtText = $"IGT: {igtText}";

                Render(igtText, TEXT_COLOR, position);
                position = position.Add(y: HEIGHT);
            }

            if (RealTimeVisible)
            {
                var rtaText = realTime.ToString(format);
                if (InGameTimeVisible)
                    rtaText = $"RTA: {rtaText}";

                Render(rtaText, TEXT_COLOR, position);
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
