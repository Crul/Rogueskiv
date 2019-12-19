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
            entities.Values.ToList().GetWithComponent<T>();

        public static List<IEntity> GetWithComponent
            (this EntityList entities, Type componentType) =>
            entities.Values.ToList().GetWithComponent(componentType);

        public static List<IEntity> GetWithComponent<T>
            (this List<IEntity> entities)
            where T : IComponent =>
            entities.Where(e => e.HasComponent<T>()).ToList();

        public static List<IEntity> GetWithComponent
            (this List<IEntity> entities, Type componentType) =>
            entities.Where(e => e.HasComponent(componentType)).ToList();
    }
}
