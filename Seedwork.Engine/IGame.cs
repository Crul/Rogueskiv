namespace Seedwork.Engine
{
    public interface IGame<T>
    {
        GameStageCode StageCode { get; }
        IGameResult<T> Result { get; }
        bool Pause { get; set; }
        bool Quit { get; }
        void Update();
        void Restart(IGameResult<T> result);
    }
}
