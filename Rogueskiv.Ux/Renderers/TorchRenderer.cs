using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Seedwork.Engine;
using Seedwork.Ux;
using System;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class TorchRenderer : PickableRenderer<TorchComp>
    {
        public TorchRenderer(IGameRenderer gameRendeerer, UxContext uxContext, IntPtr boardTexture)
            : base(
                gameRendeerer,
                uxContext,
                boardTexture,
                new SDL_Rect
                {
                    x = 5 * BoardComp.TILE_SIZE,
                    y = 2 * BoardComp.TILE_SIZE,
                    w = BoardComp.TILE_SIZE,
                    h = BoardComp.TILE_SIZE
                }
            )
        { }
    }
}
