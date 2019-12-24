namespace Rogueskiv.Core.Components
{
    public class TileFOVInfo
    {
        public bool CoveredByFOV { get; set; }
        public bool VisibleByPlayer { get; set; }
        public float DistanceFromPlayer { get; set; }
        public bool RevealedByMap { get; set; }
    }
}
