using Rogueskiv.Core.Entities;
using System.Collections.Generic;

namespace Rogueskiv.Core
{
    public interface IRenderizable
    {
        List<IEntity> Entities { get; }
    }
}
