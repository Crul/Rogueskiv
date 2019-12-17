using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Ux.Renderers;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Ux;
using System.Linq;

namespace Rogueskiv.Ux
{
    public class RogueskivRenderer : GameRenderer
    {
        private readonly IRenderizable Game;

        public RogueskivRenderer(UxContext uxContext, IRenderizable game)
            : base(uxContext)
        {
            Game = game;
            // TODO cache rendered background
            Renderers[typeof(TileComp)] = new TileRenderer(uxContext);
            Renderers[typeof(PlayerComp)] = new PlayerRenderer(uxContext);
            Renderers[typeof(EnemyComp)] = new EnemyRenderer(uxContext);
        }

        protected override void RenderGame(float interpolation) =>
            Renderers.ToList()
                .ForEach(r =>
                    r.Value.Render(Game.Entities.GetWithComponent(r.Key), interpolation)
                );
    }
}
