using Rogueskiv.Core.Components;
using Seedwork.Ux;
using System;
using System.IO;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class StairsRenderer : PositionRenderer<StairsComp>
    {
        public StairsRenderer(UxContext uxContext)
            : base(
                  uxContext,
                  Path.Combine("imgs", "stairs-down.png"),
                  new SDL_Rect { x = 0, y = 0, w = 48, h = 48 },
                  new Tuple<int, int>(30, 30)
            )
        { }
    }
}
