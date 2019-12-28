using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Seedwork.Core;
using Seedwork.Ux;
using Seedwork.Ux.SpriteProviders;
using System;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class DownStairsRenderer : FixedPositionRenderer<DownStairsComp>
    {
        public DownStairsRenderer(UxContext uxContext, IRenderizable game, IntPtr boardTexture)
            : base(
                uxContext,
                game,
                new SingleSpriteProvider<DownStairsComp>(
                    boardTexture,
                    new SDL_Rect
                    {
                        x = 0,
                        y = BoardComp.TILE_SIZE,
                        w = BoardComp.TILE_SIZE,
                        h = BoardComp.TILE_SIZE
                    }
                )
            )
        { }
    }
}
