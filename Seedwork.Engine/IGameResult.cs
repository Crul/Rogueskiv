namespace Seedwork.Engine
{
    public interface IGameResult<T>
    {
        GameResultCode ResultCode { get; }
        T Data { get; }
    }
}
