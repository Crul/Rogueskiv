using Rogueskiv.Core.Entities;
using System.Collections.Generic;

namespace Rogueskiv.Core.Systems
{
    public interface ISystem
    {
        bool Init(List<IEntity> entities);
        void Update(List<IEntity> entities);
    }
}
