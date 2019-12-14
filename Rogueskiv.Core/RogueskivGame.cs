using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Walls;
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

                    new List<IComponent> { new RightWallComp(2, 2, 4)},

                    new List<IComponent> { new DownWallComp(2, 2, 7)},
                    new List<IComponent> { new LeftWallComp(9, 2, 1)},
                    new List<IComponent> { new LeftWallComp(9, 4, 2)},
                    new List<IComponent> { new UpWallComp(2, 6, 2)},
                    new List<IComponent> { new UpWallComp(7, 6, 2)},

                    new List<IComponent> { new RightWallComp(4, 6, 6)},
                    new List<IComponent> { new LeftWallComp(7, 6, 6)},

                    new List<IComponent> { new RightWallComp(2, 12, 4)},
                    new List<IComponent> { new DownWallComp(2, 12, 2)},
                    new List<IComponent> { new DownWallComp(7, 12, 2)},
                    new List<IComponent> { new LeftWallComp(9, 12, 1)},
                    new List<IComponent> { new LeftWallComp(9, 14, 2)},
                    new List<IComponent> { new UpWallComp(2, 16, 7)},

                    new List<IComponent> { new DownWallComp(9, 3, 10)},
                    new List<IComponent> { new RightWallComp(18, 4, 1)},
                    new List<IComponent> { new UpWallComp(9, 4, 9)},

                    new List<IComponent> { new DownWallComp(10, 5, 8)},
                    new List<IComponent> { new RightWallComp(10, 5, 6)},
                    new List<IComponent> { new UpWallComp(10, 11, 1)},
                    new List<IComponent> { new UpWallComp(12, 11, 7)},
                    new List<IComponent> { new LeftWallComp(19, 3, 8)},

                    new List<IComponent> { new RightWallComp(11, 11, 1)},
                    new List<IComponent> { new LeftWallComp(12, 11, 1)},

                    new List<IComponent> { new DownWallComp(9, 13, 1)},
                    new List<IComponent> { new UpWallComp(9, 14, 1)},

                    new List<IComponent> { new RightWallComp(10, 12, 1)},
                    new List<IComponent> { new RightWallComp(10, 14, 1)},
                    new List<IComponent> { new LeftWallComp(13, 12, 3)},

                    new List<IComponent> { new DownWallComp(10, 12, 1)},
                    new List<IComponent> { new DownWallComp(12, 12, 1)},
                    new List<IComponent> { new UpWallComp(10, 15, 3)},
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
