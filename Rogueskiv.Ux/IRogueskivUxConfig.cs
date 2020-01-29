using Seedwork.Ux;

namespace Rogueskiv.Ux
{
    public interface IRogueskivUxConfig : IUxConfig
    {
        string FontFile { get; }
        int CameraMovementFriction { get; }
    }
}
