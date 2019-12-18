using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Rogueskiv.Ux.Renderers;
using Seedwork.Core;
using Seedwork.Ux;

namespace Rogueskiv.Ux
{
    public class RogueskivRenderer : GameRenderer
    {
        public RogueskivRenderer(UxContext uxContext, IRenderizable game)
            : base(uxContext, game)
        {
            // TODO cache rendered background
            Renderers[typeof(TileComp)] = new TileRenderer(uxContext);
            Renderers[typeof(PlayerComp)] = new PlayerRenderer(uxContext);
            Renderers[typeof(EnemyComp)] = new EnemyRenderer(uxContext);
        }
    }
}
