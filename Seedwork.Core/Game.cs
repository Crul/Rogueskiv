using Seedwork.Core.Components;
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
        public GameStageCode StageCode { get; protected set; } = default;
        public IGameResult Result { get; protected set; }
        public bool Pause { get; set; }
        public bool Quit { get; protected set; }

        private readonly int QuitControl;

        private int EntityIdCounter;

        public Game(
            int quitControl,
            GameStageCode stageCode = default,
            List<List<IComponent>> entitiesComponents = null,
            List<ISystem> systems = null
        )
        {
            StageCode = stageCode;
            QuitControl = quitControl;
            Entities = new EntityList();
            entitiesComponents?.ForEach(e => AddEntity(e));
            Systems = systems ?? new List<ISystem>();
            Systems = Systems.Where(sys => sys.Init(this)).ToList();
        }

        public void Update()
        {
            Quit = Controls.Any(c => c == QuitControl);
            if (!Pause)
                Systems.ForEach(s => s.Update(Entities, Controls));
        }

        public virtual void Restart()
        {
            Quit = false;
            Result = default;
        }

        public virtual void EndGame(IGameResult gameResult)
        {
            Result = gameResult;
            Quit = true;
        }

        public IEntity AddEntity(IComponent entityComponent) =>
            AddEntity(new List<IComponent> { entityComponent });

        public virtual void RemoveEntity(EntityId id) => Entities.Remove(id);

        public IEntity AddEntity(List<IComponent> entityComponents)
        {
            var entity = new Entity(new EntityId(EntityIdCounter++));
            entity.Components.AddRange(entityComponents);
            Entities.Add(entity.Id, entity);
            return entity;
        }
    }
}
