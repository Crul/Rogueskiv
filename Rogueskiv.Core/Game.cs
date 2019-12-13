using Rogueskiv.Core.Controls;
using Rogueskiv.Core.Entities;
using Rogueskiv.Core.Systems;
using Rogueskiv.Engine;
using System.Collections.Generic;
using System.Linq;

namespace Rogueskiv.Core
{
    public class Game : IGame, IControlable, IRenderizable
    {
        public IEnumerable<Control> Controls { get; set; }
        public bool Quit { get; private set; }
        public List<IEntity> Entities { get; }

        private readonly List<ISystem> Systems;
        private readonly PlayerSys PlayerSystem;

        public Game(
            List<IEntity> entities,
            List<ISystem> systems,
            PlayerSys playerSystem
        )
        {
            Entities = entities;
            Systems = systems;
            PlayerSystem = playerSystem;
        }

        public void Update()
        {
            Quit = Controls.Any(c => c == Control.QUIT);
            PlayerSystem.Update(Entities, Controls);
            Systems.ForEach(s => s.Update(Entities));
        }
    }
}
