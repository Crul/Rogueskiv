namespace Rogueskiv.Engine
{
    public interface IGameContext
    {
        int GameFPS { get; }
        long GameTicks { get; }
        int UxFPS { get; }
        long UxTicks { get; }
    }
}
