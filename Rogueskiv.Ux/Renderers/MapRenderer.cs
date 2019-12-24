using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Seedwork.Ux;
using Seedwork.Ux.SpriteProviders;
using System;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class MapRenderer : PositionRenderer<MapComp>
    {
        public MapRenderer(UxContext uxContext, IntPtr boardTexture)
            : base(
                uxContext,
                new SingleSpriteProvider<MapComp>(
                    boardTexture,
                    new SDL_Rect
                    {
                        x = 6 * BoardComp.TILE_SIZE,
                        y = 0,
                        w = BoardComp.TILE_SIZE,
                        h = BoardComp.TILE_SIZE
                    }
                )
            )
        { }
    }
}

