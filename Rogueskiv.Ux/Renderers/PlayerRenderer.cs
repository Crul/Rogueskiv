using Rogueskiv.Core.Components;
using System;
using System.IO;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class PlayerRenderer : InterpolatedPositionRenderer<PositionComp>
    {
        public PlayerRenderer(UxContext uxContext)
            : base(
                  uxContext,
                  Path.Combine("imgs", "player.png"),
                  new SDL_Rect { x = 0, y = 0, w = 48, h = 48 },
                  new Tuple<int, int>(16, 16)
            )
        { }
    }
}
