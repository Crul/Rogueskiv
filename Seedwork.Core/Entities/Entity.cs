using Seedwork.Core.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Seedwork.Core.Entities
{
    public sealed class Entity : IEntity
    {
        public EntityId Id { get; }
        public List<IComponent> Components { get; }

        public Entity(EntityId id)
        {
            Id = id;
            Components = new List<IComponent>();
        }

        public Entity AddComponent(IComponent component)
        {
            Components.Add(component);
            return this;
        }

        public bool HasComponent<T>() where T : IComponent =>
            Components.Any(c => c is T);

        public bool HasComponent(Type t) =>
            Components.Any(c => c.GetType().IsAssignableFrom(t));

        public T GetComponent<T>() where T : IComponent =>
            (T)Components.Single(c => c is T);
    }
}
