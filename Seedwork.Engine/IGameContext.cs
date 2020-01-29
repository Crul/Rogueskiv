namespace Seedwork.Engine
{
    public interface IGameContext
    {
        int GameSeed { get; }
        int GameFPS { get; }
        long GameTicks { get; }
        int UxFPS { get; }
        long UxTicks { get; }
        int MaxGameStepsWithoutRender { get; }
    }
}
