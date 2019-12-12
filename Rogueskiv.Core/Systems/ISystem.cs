using Rogueskiv.Core.Entities;
using System.Collections.Generic;

namespace Rogueskiv.Core.Systems
{
    public interface ISystem
    {
        void Update(IList<IEntity> entities);
    }
}
