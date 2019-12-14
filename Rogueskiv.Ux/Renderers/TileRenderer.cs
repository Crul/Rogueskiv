using Rogueskiv.Core.Components;
using Rogueskiv.Core.Entities;
using System;
using System.IO;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class TileRenderer : ItemRenderer<TileComp>
    {
        public TileRenderer(UxContext uxContext)
            : base(
                  uxContext,
                  Path.Combine("imgs", "tile.png"),
                  new SDL_Rect { x = 0, y = 0, w = 48, h = 48 },
                  new Tuple<int, int>(30, 30)
            )
        { }

        protected override void Render(IEntity entity, float interpolation)
        {
            var tileComp = entity.GetComponent<TileComp>();
            Render(tileComp.X, tileComp.Y);
        }
    }
}
