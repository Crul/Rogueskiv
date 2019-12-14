using Seedwork.Core.Entities;
using System.Collections.Generic;

namespace Seedwork.Core.Systems
{
    public interface ISystem
    {
        bool Init(Game game);
        void Update(EntityList entities, IEnumerable<int> controls);
    }
}
