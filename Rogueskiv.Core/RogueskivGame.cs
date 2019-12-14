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
                        new MovementComp()
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
