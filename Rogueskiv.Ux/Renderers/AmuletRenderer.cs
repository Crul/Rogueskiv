using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Seedwork.Core;
using Seedwork.Engine;
using Seedwork.Ux;
using System;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class AmuletRenderer : PickableRenderer<AmuletComp>
    {
        public AmuletRenderer(
            IGameRenderer gameRenderer,
            UxContext uxContext,
            IRenderizable game,
            IntPtr boardTexture
        )
            : base(
                gameRenderer,
                uxContext,
                game,
                boardTexture,
                new SDL_Rect
                {
                    x = 6 * BoardComp.TILE_SIZE,
                    y = BoardComp.TILE_SIZE,
                    w = BoardComp.TILE_SIZE,
                    h = BoardComp.TILE_SIZE
                }
            )
        { }
    }
}
