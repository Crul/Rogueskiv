using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Seedwork.Core;
using Seedwork.Ux;
using Seedwork.Ux.SpriteProviders;
using System;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class UpStairsRenderer : FixedPositionRenderer<UpStairsComp>
    {
        private const int UP_STAIRS_SPRITE_WIDTH = 42;
        public UpStairsRenderer(UxContext uxContext, IRenderizable game, IntPtr boardTexture)
            : base(
                uxContext,
                game,
                new SingleSpriteProvider<UpStairsComp>(
                    boardTexture,
                    new SDL_Rect
                    {
                        x = BoardComp.TILE_SIZE,
                        y = BoardComp.TILE_SIZE,
                        w = UP_STAIRS_SPRITE_WIDTH,
                        h = BoardComp.TILE_SIZE
                    }
                )
            )
        { }
    }
}
