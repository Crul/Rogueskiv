using Seedwork.Core.Entities;
using System.Collections.Generic;

namespace Seedwork.Core.Systems
{
    public abstract class BaseSystem : ISystem
    {
        public virtual bool Init(Game game) => true;

        public abstract void Update(EntityList entities, IEnumerable<int> controls);
    }
}
