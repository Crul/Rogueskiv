using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Seedwork.Core.Entities;
using Seedwork.Ux;
using System;
using System.IO;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class FoodRenderer : PositionRenderer<FoodComp>
    {
        public FoodRenderer(UxContext uxContext)
            : base(
                  uxContext,
                  Path.Combine("imgs", "food.png"),
                  new SDL_Rect { x = 0, y = 0, w = BoardComp.TILE_SIZE, h = BoardComp.TILE_SIZE },
                  new Tuple<int, int>(BoardComp.TILE_SIZE, BoardComp.TILE_SIZE)
            )
        { }

        protected override void Render(IEntity entity, float interpolation)
        {
            base.Render(entity, interpolation);
        }
    }
}
