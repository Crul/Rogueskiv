using Seedwork.Core.Entities;
using System.Collections.Generic;

namespace Seedwork.Core
{
    public interface IRenderizable
    {
        List<IEntity> Entities { get; }
    }
}
