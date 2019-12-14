using Seedwork.Core.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Seedwork.Core.Entities
{
    public static class EntityExtensions
    {
        public static List<IEntity> GetWithComponent<T>
            (this EntityList entities)
            where T : IComponent =>
            entities.Where(e => e.Value.HasComponent<T>()).Select(e => e.Value).ToList();

        public static List<IEntity> GetWithComponent
            (this EntityList entities, Type componentType) =>
            entities.Where(e => e.Value.HasComponent(componentType)).Select(e => e.Value).ToList();
    }
}
