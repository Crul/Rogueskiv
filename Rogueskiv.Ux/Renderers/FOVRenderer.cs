using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Seedwork.Core.Entities;
using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class FOVRenderer : BaseItemRenderer<FOVComp>
    {
        private const int BLACK_OPACITY_FOR_REVEALED_BY_MAP = 0xDD;
        private const int BLACK_OPACITY_FOR_COVERED_BY_FOV = 0xAA;
        private const int MAX_BLACK_OPACITY_FOR_VISIBLE = BLACK_OPACITY_FOR_COVERED_BY_FOV - 0x22;

        public FOVRenderer(UxContext uxContext) : base(uxContext) { }

        protected override void Render(IEntity entity, FOVComp fovComp, float interpolation)
        {
            fovComp.ForAllTiles(RenderTileFOV);
            SDL_SetRenderDrawColor(UxContext.WRenderer, 0, 0, 0, 0);
        }

        private void RenderTileFOV(TileFOVInfo tileFOVInfo)
        {
            if (!tileFOVInfo.CoveredByFOV && !tileFOVInfo.VisibleByPlayer)
                return;

            byte alpha;
            if (tileFOVInfo.RevealedByMap)
                alpha = BLACK_OPACITY_FOR_REVEALED_BY_MAP;
            else if (tileFOVInfo.CoveredByFOV)
                alpha = BLACK_OPACITY_FOR_COVERED_BY_FOV;
            else
                alpha = (byte)(MAX_BLACK_OPACITY_FOR_VISIBLE * tileFOVInfo.DistanceFactor);

            var screenPosition = GetScreenPosition(tileFOVInfo.Position);
            var tRect = new SDL_Rect()
            {
                x = screenPosition.X,
                y = screenPosition.Y,
                w = BoardComp.TILE_SIZE / 2,
                h = BoardComp.TILE_SIZE / 2
            };

            SDL_SetRenderDrawColor(UxContext.WRenderer, 0, 0, 0, alpha);
            SDL_RenderFillRect(UxContext.WRenderer, ref tRect);
        }
    }
}
