﻿using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Seedwork.Core;
using Seedwork.Engine;
using Seedwork.Ux;
using System;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class TorchRenderer : PickableRenderer<TorchComp>
    {
        public TorchRenderer(
            IGameRenderer gameRendeerer,
            UxContext uxContext,
            IRenderizable game,
            IntPtr boardTexture
        )
            : base(
                gameRendeerer,
                uxContext,
                game,
                boardTexture,
                new SDL_Rect
                {
                    x = BoardComp.TILE_SIZE,
                    y = 0,
                    w = BoardComp.TILE_SIZE,
                    h = BoardComp.TILE_SIZE
                }
            )
        { }
    }
}
