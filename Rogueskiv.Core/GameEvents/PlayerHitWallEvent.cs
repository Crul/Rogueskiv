namespace Rogueskiv.Core.GameEvents
{
    public class PlayerHitWallEvent : IGameEvent
    {
        public float SpeedFactor { get; }

        public PlayerHitWallEvent(float speedFactor) => SpeedFactor = speedFactor;
    }
}
