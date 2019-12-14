using Rogueskiv.Core.Entities;
using System.Collections.Generic;

namespace Rogueskiv.Core.Systems
{
    public abstract class BaseSystem : ISystem
    {
        public virtual bool Init(Game game) => true;

        public abstract void Update(List<IEntity> entities);
    }
}
