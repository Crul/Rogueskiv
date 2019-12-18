namespace Seedwork.Engine
{
    public interface IGame
    {
        GameStageCode StageCode { get; }
        IGameResult Result { get; }
        bool Pause { get; set; }
        bool Quit { get; }
        void Update();
        void Restart();
    }
}
