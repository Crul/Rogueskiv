using System.Collections.Generic;

namespace Seedwork.Engine
{
    public interface IGameResult<T>
    {
        GameResultCode ResultCode { get; }
        List<T> Data { get; }
    }
}
