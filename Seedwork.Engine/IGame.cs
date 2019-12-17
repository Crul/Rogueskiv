namespace Seedwork.Engine
{
    public interface IGame
    {
        bool Pause { get; set; }
        bool Quit { get; }
        void Init();
        void Update();
    }
}
