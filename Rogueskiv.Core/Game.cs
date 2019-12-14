using Rogueskiv.Core.Components;
using Rogueskiv.Core.Controls;
using Rogueskiv.Core.Entities;
using Rogueskiv.Core.Systems;
using Rogueskiv.Engine;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core
{
    public class Game : IGame, IControlable, IRenderizable
    {
        public IEnumerable<Control> Controls { get; set; }
        public bool Quit { get; private set; }
        public List<IEntity> Entities { get; }

        private List<ISystem> Systems;
        private readonly PlayerSys PlayerSystem;
        private int EntityIdCounter;

        public Game(
            List<List<IComponent>> entitiesComponents,
            List<ISystem> systems,
            PlayerSys playerSystem
        )
        {
            Entities = new List<IEntity>();
            entitiesComponents.ForEach(AddEntity);
            Systems = systems;
            PlayerSystem = playerSystem;
        }

        public void Init() =>
            Systems = Systems.Where(sys => sys.Init(this)).ToList();

        public void Update()
        {
            Quit = Controls.Any(c => c == Control.QUIT);
            PlayerSystem.Update(Entities, Controls);
            Systems.ForEach(s => s.Update(Entities));
        }

        public void AddEntity(IComponent entityComponent) =>
            AddEntity(new List<IComponent> { entityComponent });

        public void AddEntity(List<IComponent> entityComponents)
        {
            var entity = new Entity(new EntityId(EntityIdCounter++));
            entity.Components.AddRange(entityComponents);
            Entities.Add(entity);
        }
    }
}
