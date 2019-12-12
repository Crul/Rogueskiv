using Rogueskiv.Core.Components;
using Rogueskiv.Core.Entities;
using System.IO;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class PlayerRenderer : ItemRenderer
    {
        public PlayerRenderer(UxContext uxContext)
            : base(
                  uxContext,
                  Path.Combine("imgs", "player.png"),
                  new SDL_Rect { x = 0, y = 0, w = 48, h = 48 }
            )
        { }

        protected override void Render(IEntity entity)
        {
            if (!entity.HasComponent<PositionComp>())
                return;

            var positionComp = entity.GetComponent<PositionComp>();
            Render(positionComp.X, positionComp.Y);
        }
    }
}
