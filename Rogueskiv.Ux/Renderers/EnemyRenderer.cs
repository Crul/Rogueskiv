﻿using Rogueskiv.Core.Components.Position;
using Seedwork.Ux;
using System;
using System.IO;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class EnemyRenderer : InterpolatedPositionRenderer<CurrentPositionComp>
    {
        public EnemyRenderer(UxContext uxContext)
            : base(
                  uxContext,
                  Path.Combine("imgs", "enemy.png"),
                  new SDL_Rect { x = 0, y = 0, w = 48, h = 48 },
                  new Tuple<int, int>(12, 12)
            )
        { }
    }
}