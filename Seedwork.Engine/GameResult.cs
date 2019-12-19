namespace Seedwork.Engine
{
    public class GameResult : IGameResult
    {
        public GameResultCode ResultCode { get; }

        public GameResult(int resultCode) =>
            ResultCode = new GameResultCode(resultCode);
    }
}
