﻿using Seedwork.Core.Components;
using Seedwork.Core.Controls;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using Seedwork.Engine;
using System.Collections.Generic;
using System.Linq;

namespace Seedwork.Core
{
    public class Game : IGame, IControlable, IRenderizable
    {
        public EntityList Entities { get; }

        private List<ISystem> Systems;
        public IEnumerable<int> Controls { get; set; }
        public bool Quit { get; private set; }

        private readonly int QuitControl;

        private int EntityIdCounter;

        public Game(
            List<List<IComponent>> entitiesComponents,
            List<ISystem> systems,
            int quitControl
        )
        {
            Entities = new EntityList();
            entitiesComponents.ForEach(e => AddEntity(e));
            Systems = systems;
            QuitControl = quitControl;
        }

        public void Init() =>
            Systems = Systems.Where(sys => sys.Init(this)).ToList();

        public void Update()
        {
            Quit = Controls.Any(c => c == QuitControl);
            Systems.ForEach(s => s.Update(Entities, Controls));
        }

        public IEntity AddEntity(IComponent entityComponent) =>
            AddEntity(new List<IComponent> { entityComponent });

        public IEntity AddEntity(List<IComponent> entityComponents)
        {
            var entity = new Entity(new EntityId(EntityIdCounter++));
            entity.Components.AddRange(entityComponents);
            Entities.Add(entity.Id, entity);
            return entity;
        }
    }
}
