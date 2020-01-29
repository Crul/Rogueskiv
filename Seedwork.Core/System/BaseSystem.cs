using Seedwork.Core.Entities;
using System.Collections.Generic;

namespace Seedwork.Core.Systems
{
    public abstract class BaseSystem : ISystem
    {
        public virtual void Init(Game game) { }

        public abstract void Update(EntityList entities, List<int> controls);
    }
}
