using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Entities
{
    public static class EntityExtensions
    {
        public static List<IEntity> GetWithComponent
        (
            this List<IEntity> entities, Type componentType
        ) => entities.Where(e => e.HasComponent(componentType)).ToList();
    }
}
