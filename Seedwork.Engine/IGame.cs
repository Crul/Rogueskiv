namespace Seedwork.Engine
{
    public interface IGame
    {
        GameStageCode StageCode { get; }
        IGameResult Result { get; set; }
        bool Pause { get; set; }
        bool Quit { get; }
        void Init();
        void Update();
    }
}
