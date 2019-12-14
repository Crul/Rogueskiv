using Rogueskiv.Core.Components.Board;
using Seedwork.Ux;
using System;
using System.IO;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class TileRenderer : PositionRenderer<TileComp>
    {
        public TileRenderer(UxContext uxContext)
            : base(
                  uxContext,
                  Path.Combine("imgs", "tile.png"),
                  new SDL_Rect { x = 0, y = 0, w = 48, h = 48 },
                  new Tuple<int, int>(30, 30)
            )
        { }
    }
}
