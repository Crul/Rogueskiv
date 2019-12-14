using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Rogueskiv.Core.Systems;
using Seedwork.Core;
using Seedwork.Core.Components;
using Seedwork.Core.Systems;
using Seedwork.Engine;
using System.Collections.Generic;

namespace Rogueskiv.Core
{
    public class RogueskivGame : Game
    {
        public RogueskivGame(IGameContext gameContext, string boardData)
            : base(
                entitiesComponents: new List<List<IComponent>>()
                {
                    new List<IComponent> {
                        new PlayerComp(),
                        new CurrentPositionComp(){ X = 90, Y = 90 },
                        new LastPositionComp(),
                        new MovementComp(){
                            FrictionFactor = 1f / 5f,
                            BounceAmortiguationFactor = 2f / 3f
                        }
                    },
                    new List<IComponent>
                    {
                        new EnemyComp(),
                        new CurrentPositionComp(){ X = 120, Y = 120 },
                        new LastPositionComp(),
                        new MovementComp() {
                            SpeedX = 150f / gameContext.GameFPS,
                            SpeedY = 50f / gameContext.GameFPS,
                            FrictionFactor = 1,
                            BounceAmortiguationFactor = 1
                        }
                    },
                    new List<IComponent>
                    {
                        new EnemyComp(),
                        new CurrentPositionComp(){ X = 125, Y = 120 },
                        new LastPositionComp(),
                        new MovementComp() {
                            SpeedX = 50f / gameContext.GameFPS,
                            SpeedY = 150f / gameContext.GameFPS,
                            FrictionFactor = 1,
                            BounceAmortiguationFactor = 1
                        }
                    },
                    new List<IComponent> { new BoardComp(boardData) },
                },
                systems: new List<ISystem> {
                    new PlayerSys(gameContext),
                    new MovementSys(),
                    new WallSys(),
                    new BoardSys(),
                },
                quitControl: (int)Core.Controls.QUIT
            )
        { }
    }
}
