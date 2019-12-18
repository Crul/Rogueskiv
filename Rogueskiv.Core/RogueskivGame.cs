﻿using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.Systems;
using Seedwork.Core;
using Seedwork.Core.Components;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using Seedwork.Engine;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core
{
    public class RogueskivGame : Game
    {
        private readonly BoardComp BoardComp;

        public RogueskivGame(
            IGameContext gameContext,
            string boardData,
            GameStageCode stageCode
        )
            : base(
                quitControl: (int)Core.Controls.QUIT,
                stageCode: stageCode,
                entitiesComponents: new List<List<IComponent>>
                {
                    new List<IComponent> { new BoardComp(boardData) }
                },
                systems: new List<ISystem> {
                    new BoardSys(),
                    new SpawnSys(gameContext),
                    new PlayerSys(gameContext),
                    new MovementSys(),
                    new WallSys(),
                    new CollisionSys(),
                    new FOVSys(),
                    new DeathSys()
                }
            ) => BoardComp = Entities
                    .GetWithComponent<BoardComp>()
                    .Single()
                    .GetComponent<BoardComp>();

        public override void RemoveEntity(EntityId id)
        {
            var position = Entities[id].GetComponent<CurrentPositionComp>();
            BoardComp.RemoveEntity(id, position);
            base.RemoveEntity(id);
        }
    }
}
