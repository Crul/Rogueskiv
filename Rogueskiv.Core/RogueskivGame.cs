using Rogueskiv.Core.Components;
using Rogueskiv.Core.Entities;
using Rogueskiv.Core.Systems;
using System.Collections.Generic;

namespace Rogueskiv.Core
{
    public class RogueskivGame : Game
    {
        public RogueskivGame()
            : base(
                entities: new List<IEntity>()
                {
                    new Entity(new EntityId(1))
                        .AddComponent(new PlayerComp())
                        .AddComponent(new PositionComp())
                        .AddComponent(new MovementComp())
                },
                systems: new List<ISystem>() { new MovementSys() },
                playerSystem: new PlayerSys()
            )
        { }
    }
}
