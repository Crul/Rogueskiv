using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Seedwork.Engine;
using Seedwork.Ux;
using System;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class MapRenderer : PickableRenderer<MapComp>
    {
        public MapRenderer(IGameRenderer gameRenderer, UxContext uxContext, IntPtr boardTexture)
            : base(
                gameRenderer,
                uxContext,
                boardTexture,
                new SDL_Rect
                {
                    x = 6 * BoardComp.TILE_SIZE,
                    y = 0,
                    w = BoardComp.TILE_SIZE,
                    h = BoardComp.TILE_SIZE
                }
            )
        { }
    }
}

