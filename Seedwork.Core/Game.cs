﻿using Seedwork.Core.Components;
using Seedwork.Core.Controls;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using Seedwork.Engine;
using System.Collections.Generic;
using System.Linq;

namespace Seedwork.Core
{
    public class Game : IGame<EntityList>, IControlable, IRenderizable
    {
        public EntityList Entities { get; }
        private List<ISystem> Systems { get; }
        public List<int> Controls { get; set; }
        public GameStageCode StageCode { get; protected set; } = default;
        public IGameResult<EntityList> Result { get; protected set; }

        public bool Pause { get; set; }
        public bool Quit { get; protected set; }

        private bool PauseControlPressedBefore = false;
        protected readonly int PauseControl;
        protected readonly int QuitControl;
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
            Systems = new List<ISystem>();
            if (systems != null)
                systems.ForEach(AddSystem);
        }

        public virtual void Update()
        {
            Quit = Quit || Controls.Contains(QuitControl);
            SetPause();

            if (!Pause && !Quit)
                UpdateSystems();
        }

        protected virtual void UpdateSystems()
            => Systems.ToList().ForEach(s => s.Update(Entities, Controls));

        public virtual void Restart(IGameResult<EntityList> result)
        {
            Quit = false;
            Result = default;
        }

        public virtual void EndGame(IGameResult<EntityList> gameResult, bool pauseBeforeQuit = false)
        {
            Result = gameResult;
            if (pauseBeforeQuit)
                Pause = true;
            else
                Quit = true;
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

        public virtual void RemoveEntity(EntityId id) => Entities.Remove(id);

        protected void AddSystem(ISystem system)
        {
            Systems.Add(system);
            system.Init(this);
        }

        public void RemoveSystem(ISystem system) => Systems.Remove(system);

        private void SetPause()
        {
            var pausePressedNow = Controls.Contains(PauseControl);
            if (!PauseControlPressedBefore && pausePressedNow)
                Pause = !Pause;

            PauseControlPressedBefore = pausePressedNow;
        }
    }
}
