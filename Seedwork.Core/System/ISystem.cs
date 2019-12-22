using Seedwork.Core.Entities;
using System.Collections.Generic;

namespace Seedwork.Core.Systems
{
    public interface ISystem
    {
        void Init(Game game);
        void Update(EntityList entities, List<int> controls);
    }
}
