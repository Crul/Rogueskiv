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
        public int InGameTime { get; set; }
    }
}
