﻿using Rogueskiv.Core.Components;
using Seedwork.Ux;
using System;
using System.IO;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class UpStairsRenderer : PositionRenderer<StairsComp>
    {
        public UpStairsRenderer(UxContext uxContext)
            : base(
                  uxContext,
                  Path.Combine("imgs", "stairs-up.png"),
                  new SDL_Rect { x = 0, y = 0, w = 48, h = 48 },
                  new Tuple<int, int>(30, 30)
            )
        { }
    }
}
