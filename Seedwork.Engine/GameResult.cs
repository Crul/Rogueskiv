namespace Seedwork.Engine
{
    public class GameResult<T> : IGameResult<T>
        where T : new()
    {
        public GameResultCode ResultCode { get; }
        public T Data { get; }

        public GameResult(int resultCode)
        {
            ResultCode = new GameResultCode(resultCode);
            Data = new T();
        }
    }
}
