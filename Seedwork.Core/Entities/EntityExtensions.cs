using Seedwork.Core.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Seedwork.Core.Entities
{
    public static class EntityExtensions
    {
        public static T GetSingleComponent<T>
            (this EntityList entities)
            where T : IComponent =>
            entities.Values.GetSingleComponent<T>();

        public static TComp GetSingleComponent<TWithComp, TComp>
            (this EntityList entities)
            where TWithComp : IComponent
            where TComp : IComponent =>
            entities.Values.GetSingleComponent<TWithComp, TComp>();

        public static List<T> GetComponents<T>
            (this EntityList entities)
            where T : IComponent =>
            entities.Values.GetComponents<T>();

        public static List<IEntity> GetWithComponent<T>
            (this EntityList entities)
            where T : IComponent =>
            entities.Values.GetWithComponent<T>();

        public static List<IEntity> GetWithComponent
            (this EntityList entities, Type componentType) =>
            entities.Values.GetWithComponent(componentType);

        public static T GetSingleComponent<T>
            (this IEnumerable<IEntity> entities)
            where T : IComponent =>
            entities
                .GetWithComponent<T>()
                .Single()
                .GetComponent<T>();

        public static List<T> GetComponents<T>
            (this IEnumerable<IEntity> entities)
            where T : IComponent =>
            entities
                .GetWithComponent<T>()
                .Select(e => e.GetComponent<T>())
                .ToList();

        public static TComp GetSingleComponent<TWithComp, TComp>
            (this IEnumerable<IEntity> entities)
            where TWithComp : IComponent
            where TComp : IComponent =>
            entities
                .GetWithComponent<TWithComp>()
                .Single()
                .GetComponent<TComp>();

        public static List<IEntity> GetWithComponent<T>
            (this IEnumerable<IEntity> entities)
            where T : IComponent =>
            entities.Where(e => e.HasComponent<T>()).ToList();

        public static List<IEntity> GetWithComponent
            (this IEnumerable<IEntity> entities, Type componentType) =>
            entities.Where(e => e.HasComponent(componentType)).ToList();
    }
}
