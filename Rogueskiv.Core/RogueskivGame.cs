using Rogueskiv.Core.Components;
using Rogueskiv.Core.Systems;
using Rogueskiv.Engine;
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
                        new PositionComp(){ X = 90, Y = 90 },
                        new MovementComp()
                    },
                    new List<IComponent> { new BoardComp(boardData) },
                },
                systems: new List<ISystem> {
                    new MovementSys(),
                    new BoardSys(),
                },
                playerSystem: new PlayerSys(gameContext)
            )
        { }
    }
}
