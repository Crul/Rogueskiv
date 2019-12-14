namespace Seedwork.Engine
{
    public interface IGame
    {
        bool Quit { get; }
        void Init();
        void Update();
    }
}
