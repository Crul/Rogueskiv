using Rogueskiv.Core.Entities;
using System.Collections.Generic;

namespace Rogueskiv.Core.Systems
{
    public interface ISystem
    {
        bool Init(Game game);
        void Update(List<IEntity> entities);
    }
}
