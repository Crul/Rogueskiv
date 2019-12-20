using Seedwork.Core.Components;
using Seedwork.Core.Controls;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using Seedwork.Engine;
using System.Collections.Generic;
using System.Linq;

namespace Seedwork.Core
{
    public class Game : IGame<IEntity>, IControlable, IRenderizable
    {
        public EntityList Entities { get; }

        private List<ISystem> Systems;
        public List<int> Controls { get; set; }
        public GameStageCode StageCode { get; protected set; } = default;
        public IGameResult<IEntity> Result { get; protected set; }
        public bool Pause { get; set; }
        public bool Quit { get; protected set; }

        private bool PauseControlPressedBefore = false;
        private readonly int PauseControl;
        private readonly int QuitControl;
        private int EntityIdCounter;

        public Game(
            GameStageCode stageCode = default,
            List<List<IComponent>> entitiesComponents = null,
            List<ISystem> systems = null,
            int pauseControl = -1,
            int quitControl = -1
        )
        {
            StageCode = stageCode;
            PauseControl = pauseControl;
            QuitControl = quitControl;
            Entities = new EntityList();
            entitiesComponents?.ForEach(e => AddEntity(e));
            Systems = systems ?? new List<ISystem>();
            Systems = Systems.Where(sys => sys.Init(this)).ToList();
        }

        public void Update()
        {
            Quit = Controls.Contains(QuitControl);
            SetPause();

            if (!Pause && !Quit)
                Systems.ForEach(s => s.Update(Entities, Controls));
        }

        public virtual void Restart(IGameResult<IEntity> result)
        {
            Quit = false;
            Result = default;
        }

        public virtual void EndGame(IGameResult<IEntity> gameResult)
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

        private void SetPause()
        {
            var pausePressedNow = Controls.Contains(PauseControl);
            if (!PauseControlPressedBefore && pausePressedNow)
                Pause = !Pause;

            PauseControlPressedBefore = pausePressedNow;
        }
    }
}
