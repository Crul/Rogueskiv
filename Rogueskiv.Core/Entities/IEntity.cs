using Rogueskiv.Core.Components;
using System;

namespace Rogueskiv.Core.Entities
{
    public interface IEntity
    {
        EntityId Id { get; }
        T GetComponent<T>() where T : IComponent;
        bool HasComponent<T>() where T : IComponent;
        bool HasComponent(Type componentType);
    }
}
