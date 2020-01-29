using System;

namespace Rogueskiv.Core
{
    public class RogueskivGameStats
    {
        public long Timestamp { get; set; }
        public string GameMode { get; set; }
        public int Floors { get; set; }
        public int? DiedOnFloor { get; set; }
        public int FinalHealth { get; set; }
        public long RealTime { get; set; }
        public long InGameTime { get; set; }

        public string GetResult() => DiedOnFloor.HasValue ? "DEAD" : "WIN";

        public string GetDateTime()
        {
            var dateTime = new DateTime(Timestamp);

            return $"{dateTime.ToString("dd/MM")} {dateTime.ToShortTimeString()}";
        }

        public string GetRealTimeFormatted() => new TimeSpan(RealTime).ToString("g");

        public string GetInGameTimeFormatted() => new TimeSpan(InGameTime).ToString("g");
    }
}
