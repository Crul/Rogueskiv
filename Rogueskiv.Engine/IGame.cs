namespace Rogueskiv.Engine
{
    public interface IGame
    {
        bool Quit { get; }
        void Init();
        void Update();
    }
}
