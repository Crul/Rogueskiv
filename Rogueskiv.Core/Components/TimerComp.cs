using Seedwork.Core.Components;
using System;

namespace Rogueskiv.Core.Components
{
    public class TimerComp : IComponent
    {
        public bool HasStarted { get; private set; }
        public DateTime? RealTimeStart { get; private set; }
        public DateTime? RealTimeStop { get; private set; }
        public long InGameTime { get; set; }

        public TimerComp(TimerComp previousTimer)
        {
            RealTimeStart = previousTimer?.RealTimeStart;
            HasStarted = RealTimeStart.HasValue;
            InGameTime = previousTimer?.InGameTime ?? 0;
        }

        public TimeSpan GetRealTime() => RealTimeStart.HasValue
            ? FinalTime - RealTimeStart.Value
            : new TimeSpan();

        public void Start()
        {
            if (HasStarted)
                throw new Exception("TimerComp started twice");

            HasStarted = true;
            RealTimeStart = DateTime.Now;
        }

        public void Stop() => RealTimeStop = DateTime.Now;

        private DateTime FinalTime => RealTimeStop ?? DateTime.Now;
    }
}
