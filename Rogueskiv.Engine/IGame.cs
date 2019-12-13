namespace Rogueskiv.Engine
{
    public interface IGame
    {
        bool Quit { get; }
        void Update();
    }
}
