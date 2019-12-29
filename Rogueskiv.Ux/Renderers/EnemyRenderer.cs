using Rogueskiv.Core.Components.Position;
using Seedwork.Ux;
using Seedwork.Ux.SpriteProviders;
using System.IO;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class EnemyRenderer : InterpolatedPositionRenderer<CurrentPositionComp>
    {
        public EnemyRenderer(UxContext uxContext)
            : base(
                uxContext,
                new SingleSpriteProvider<CurrentPositionComp>(
                    uxContext,
                    Path.Combine("imgs", "enemy.png"),
                    new SDL_Rect { x = 0, y = 0, w = 16, h = 16 }
                )
            )
        { }
    }
}
