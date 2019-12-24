using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Seedwork.Ux;
using Seedwork.Ux.SpriteProviders;
using System;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class TorchRenderer : PositionRenderer<TorchComp>
    {
        public TorchRenderer(UxContext uxContext, IntPtr boardTexture)
            : base(
                uxContext,
                new SingleSpriteProvider<TorchComp>(
                    boardTexture,
                    new SDL_Rect
                    {
                        x = 5 * BoardComp.TILE_SIZE,
                        y = 2 * BoardComp.TILE_SIZE,
                        w = BoardComp.TILE_SIZE,
                        h = BoardComp.TILE_SIZE
                    }
                )
            )
        { }
    }
}
