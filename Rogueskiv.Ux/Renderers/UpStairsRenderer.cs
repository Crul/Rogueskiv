using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Seedwork.Ux;
using System;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class UpStairsRenderer : PositionRenderer<StairsComp>
    {
        public UpStairsRenderer(UxContext uxContext, IntPtr boardTexture)
            : base(
                  uxContext,
                  boardTexture,
                  new SDL_Rect { x = 2 * BoardComp.TILE_SIZE, y = 0, w = BoardComp.TILE_SIZE, h = BoardComp.TILE_SIZE },
                  new Tuple<int, int>(BoardComp.TILE_SIZE, BoardComp.TILE_SIZE)
            )
        { }
    }
}
