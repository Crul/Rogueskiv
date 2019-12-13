using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Walls;
using Rogueskiv.Core.Entities;
using Rogueskiv.Core.Systems;
using Rogueskiv.Engine;
using System.Collections.Generic;

namespace Rogueskiv.Core
{
    public class RogueskivGame : Game
    {
        private static int EntityIdCounter = 0;

        public RogueskivGame(IGameContext gameContext)
            : base(
                entities: new List<IEntity>()
                {
                    new Entity(new EntityId(EntityIdCounter++))
                        .AddComponent(new PlayerComp())
                        .AddComponent(new PositionComp(){ X = 90, Y = 90 })
                        .AddComponent(new MovementComp()),

                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new RightWallComp(2, 2, 4)),

                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new DownWallComp(2, 2, 7)),
                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new LeftWallComp(9, 2, 1)),
                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new LeftWallComp(9, 4, 2)),
                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new UpWallComp(2, 6, 2)),
                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new UpWallComp(7, 6, 2)),

                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new RightWallComp(4, 6, 6)),
                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new LeftWallComp(7, 6, 6)),

                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new RightWallComp(2, 12, 4)),
                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new DownWallComp(2, 12, 2)),
                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new DownWallComp(7, 12, 2)),
                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new LeftWallComp(9, 12, 1)),
                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new LeftWallComp(9, 14, 2)),
                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new UpWallComp(2, 16, 7)),

                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new DownWallComp(9, 3, 10)),
                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new RightWallComp(18, 4, 1)),
                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new UpWallComp(9, 4, 9)),

                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new DownWallComp(10, 5, 8)),
                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new RightWallComp(10, 5, 6)),
                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new UpWallComp(10, 11, 1)),
                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new UpWallComp(12, 11, 7)),
                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new LeftWallComp(19, 3, 8)),

                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new RightWallComp(11, 11, 1)),
                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new LeftWallComp(12, 11, 1)),

                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new DownWallComp(9, 13, 1)),
                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new UpWallComp(9, 14, 1)),

                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new RightWallComp(10, 12, 1)),
                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new RightWallComp(10, 14, 1)),
                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new LeftWallComp(13, 12, 3)),

                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new DownWallComp(10, 12, 1)),
                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new DownWallComp(12, 12, 1)),
                    new Entity(new EntityId(EntityIdCounter++)).AddComponent(new UpWallComp(10, 15, 3)),
                },
                systems: new List<ISystem>() { new MovementSys() },
                playerSystem: new PlayerSys(gameContext)
            )
        { }
    }
}
