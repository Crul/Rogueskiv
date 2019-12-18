using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Seedwork.Core.Entities;
using Seedwork.Ux;
using System;
using System.IO;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class FOVRenderer : PositionRenderer<IFOVComp>
    {
        private const int MAX_BLACK_OPACITY = 0xAA;
        private const int MAX_BLACK_OPACITY_FOR_VISIBLE = MAX_BLACK_OPACITY - 0x44;
        private const int TILE_SIZE = 30;    // TODO proper tile size
        private const int VISUAL_RANGE = 10; // TODO proper visual range

        public FOVRenderer(UxContext uxContext)
            : base(
                  uxContext,
                  Path.Combine("imgs", "tile.png"),  // TODO not needed
                  new SDL_Rect { x = 0, y = 0, w = 48, h = 48 },
                  new Tuple<int, int>(30, 30)
            )
        { }

        protected override void Render(IEntity entity, float interpolation)
        {
            // TODO DRY PositionRenderer / ItemRender
            var positionComp = entity.GetComponent<TileComp>();

            byte alpha;
            if (positionComp.Visible && !positionComp.VisibleByPlayer)
                alpha = MAX_BLACK_OPACITY;
            else
                alpha = (byte)(
                    MAX_BLACK_OPACITY_FOR_VISIBLE
                    * (positionComp.DistanceFromPlayer / (TILE_SIZE * VISUAL_RANGE))
                );

            var (posX, posY) = GetXY(entity, positionComp, interpolation);

            var x = GetPositionComponent(posX, UxContext.CenterX);
            var y = GetPositionComponent(posY, UxContext.CenterY);
            var tRect = new SDL_Rect()
            {
                x = x - OutputSize.Item1 / 2,
                y = y - OutputSize.Item2 / 2,
                w = OutputSize.Item1,
                h = OutputSize.Item2
            };

            SDL_SetRenderDrawColor(UxContext.WRenderer, 0, 0, 0, alpha);
            SDL_RenderFillRect(UxContext.WRenderer, ref tRect);
            SDL_SetRenderDrawColor(UxContext.WRenderer, 0, 0, 0, 0);
        }
    }
}
