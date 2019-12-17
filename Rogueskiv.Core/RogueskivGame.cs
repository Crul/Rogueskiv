using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.Systems;
using Seedwork.Core;
using Seedwork.Core.Components;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using Seedwork.Engine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core
{
    public class RogueskivGame : Game
    {
        private BoardComp BoardComp;

        public RogueskivGame(IGameContext gameContext, string boardData)
            : base(
                entitiesComponents: new List<List<IComponent>>()
                {
                    new List<IComponent> {
                        new PlayerComp(),
                        new HealthComp() { Health = 100 },
                        new CurrentPositionComp(){ X = 90, Y = 90 },
                        new LastPositionComp(),
                        new MovementComp(){
                            FrictionFactor = 1f / 5f,
                            BounceAmortiguationFactor = 2f / 3f
                        }
                    },
                    new List<IComponent> { new BoardComp(boardData) },
                },
                systems: new List<ISystem> {
                    new PlayerSys(gameContext),
                    new MovementSys(),
                    new WallSys(),
                    new BoardSys(),
                    new CollisionSys(),
                    new DeathSys()
                },
                quitControl: (int)Core.Controls.QUIT
            )
        {
            BoardComp = Entities
                .GetWithComponent<BoardComp>()
                .Single()
                .GetComponent<BoardComp>();

            var rnd = new Random();
            for (var i = 0; i < 3; i++)
                for (var j = 0; j < 4; j++)
                    AddEntity(new List<IComponent>
                    {
                        new EnemyComp(),
                        new CurrentPositionComp(){ X = 315 + i*40, Y = 165 + j*20 },
                        new LastPositionComp(),
                        new MovementComp() {
                            SpeedX = (50 + rnd.Next(100)) / gameContext.GameFPS,
                            SpeedY = (50 + rnd.Next(100)) / gameContext.GameFPS,
                            FrictionFactor = 1,
                            BounceAmortiguationFactor = 1
                        }
                    }
                );
        }

        public override void RemoveEntity(EntityId id)
        {
            var position = Entities[id].GetComponent<CurrentPositionComp>();
            BoardComp.RemoveEntity(id, position);
            base.RemoveEntity(id);
        }
    }
}
