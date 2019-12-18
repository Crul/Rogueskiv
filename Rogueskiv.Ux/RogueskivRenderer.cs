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
            Renderers[typeof(TileComp)] = new TileRenderer(uxContext);
            Renderers[typeof(StairsComp)] = new StairsRenderer(uxContext);
            Renderers[typeof(EnemyComp)] = new EnemyRenderer(uxContext);
            Renderers[typeof(IFOVComp)] = new FOVRenderer(uxContext);
            Renderers[typeof(PlayerComp)] = new PlayerRenderer(uxContext);
        }
    }
}
