﻿using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Position;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core.Systems
{
    abstract class PickingSys<T> : BaseSystem
        where T : PickableComp
    {
        protected RogueskivGame Game { get; private set; }

        private PlayerComp PlayerComp;
        private PositionComp PlayerPositionComp;
        private readonly bool IsSingleCompPerFloor;

        protected PickingSys(bool isSingleCompPerFloor) =>
            IsSingleCompPerFloor = isSingleCompPerFloor;

        public override void Init(Game game)
        {
            Game = (RogueskivGame)game;
            var playerEntity = game.Entities.GetWithComponent<PlayerComp>().Single();
            PlayerComp = playerEntity.GetComponent<PlayerComp>();
            PlayerPositionComp = playerEntity.GetComponent<CurrentPositionComp>();
        }

        public override void Update(EntityList entities, List<int> controls)
        {
            var pickableEntities = entities
                .GetWithComponent<T>()
                .Select(e => (
                    entityId: e.Id,
                    pickableComp: e.GetComponent<T>()
                ));

            if (CanIPick())
                CheckNewPicked(pickableEntities);

            CheckBeingPicked(entities, pickableEntities);
        }

        private void CheckNewPicked(IEnumerable<(EntityId entityId, T pickableComp)> pickableEntities)
        {
            var pickedComponents = pickableEntities
                .Where(pickable =>
                    !pickable.pickableComp.IsBeingPicked
                    && pickable.pickableComp.TilePos == PlayerPositionComp.TilePos
                )
                .Select(picked => picked.pickableComp)
                .ToList();

            if (pickedComponents.Count > 0)
                StartPicking(pickedComponents);
        }

        private void CheckBeingPicked(
            EntityList entities,
            IEnumerable<(EntityId entityId, T pickableComp)> pickableEntities
        )
        {
            if (!pickableEntities.Any())
                return;

            pickableEntities
                .Where(pickable => pickable.pickableComp.IsBeingPicked)
                .ToList()
                .ForEach(picked => picked.pickableComp.TickPickingTime());

            var finallyPickedEntities = pickableEntities
                .Where(pickable => pickable.pickableComp.PickingTime < 0)
                .ToList();

            if (finallyPickedEntities.Count > 0)
            {
                EndPicking(entities, finallyPickedEntities);

                if (IsSingleCompPerFloor)
                    Game.RemoveSystem(this);
            }
        }

        protected virtual bool CanIPick() => true;

        protected virtual void StartPicking(List<T> pickedComponents) =>
            pickedComponents.ForEach(pickedComp =>
            {
                pickedComp.StartPicking();
                PlayerComp.PickingComps.Add(pickedComp);
                var gameEvent = pickedComp.GetGameEvent();
                if (gameEvent != null)
                    Game.GameEvents.Add(gameEvent);
            });

        protected virtual void EndPicking(
            EntityList entities,
            List<(EntityId entityId, T pickableComp)> pickedEntities
        )
        {
            pickedEntities.ForEach(picked =>
            {
                PlayerComp.PickingComps.Remove(picked.pickableComp);
                entities.Remove(picked.entityId);
            });
        }
    }
}
