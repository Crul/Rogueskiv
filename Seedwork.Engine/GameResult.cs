using System.Collections.Generic;

namespace Seedwork.Engine
{
    public class GameResult<T> : IGameResult<T>
    {
        public GameResultCode ResultCode { get; }
        public List<T> Data { get; }

        public GameResult(int resultCode)
        {
            ResultCode = new GameResultCode(resultCode);
            Data = new List<T>();
        }
    }
}
