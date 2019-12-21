using Rogueskiv.Core.Components.Board;
using Seedwork.Ux;
using System;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class TileRenderer : PositionRenderer<TileComp>
    {
        public TileRenderer(UxContext uxContext, IntPtr boardTexxture)
            : base(
                  uxContext,
                  boardTexxture,
                  new SDL_Rect { x = 0, y = 0, w = BoardComp.TILE_SIZE, h = BoardComp.TILE_SIZE },
                  new Tuple<int, int>(BoardComp.TILE_SIZE, BoardComp.TILE_SIZE)
            )
        { }
    }
}
