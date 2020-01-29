using Rogueskiv.Core.Components.Position;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Ux;
using Seedwork.Ux.SpriteProviders;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class EnemyRenderer : InterpolatedPositionRenderer<CurrentPositionComp>
    {
        public EnemyRenderer(UxContext uxContext, IRenderizable game)
            : base(
                uxContext,
                game,
                new SingleSpriteProvider<CurrentPositionComp>(
                    uxContext,
                    "enemy.png",
                    new SDL_Rect { x = 0, y = 0, w = 16, h = 16 }
                )
            )
        { }

        protected override void Render(IEntity entity, CurrentPositionComp positionComp, float interpolation)
        {
            if (FOVComp.IsVisibleByPlayer(positionComp))
                base.Render(entity, positionComp, interpolation);
        }
    }
}
