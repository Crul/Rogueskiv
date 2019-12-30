namespace Rogueskiv.Ux
{
    public interface IRogueskivUxConfig
    {
        string FontFile { get; }
        int CameraMovementFriction { get; }
        int PlayerRadius { get; }
        bool InGameTimeVisible { get; }
        bool RealTimeVisible { get; }
    }
}
