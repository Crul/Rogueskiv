using Seedwork.Core.Entities;
using Seedwork.Engine;

namespace Seedwork.Core
{
    public interface IRenderizable
    {
        EntityList Entities { get; }
        bool Pause { get; }
        IGameResult<IEntity> Result { get; }
    }
}
