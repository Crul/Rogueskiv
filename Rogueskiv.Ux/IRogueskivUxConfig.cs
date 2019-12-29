namespace Rogueskiv.Ux
{
    public interface IRogueskivUxConfig
    {
        string FontFile { get; }
        int CameraMovementFriction { get; }
        int PlayerRadius { get; }
    }
}
