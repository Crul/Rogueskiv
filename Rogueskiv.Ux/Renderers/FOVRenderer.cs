using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Board;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Crosscutting;
using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using System.Drawing;
using System.Linq;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class FOVRenderer : BaseItemRenderer<FOVComp>
    {
        private const int MAX_BLACK_OPACITY = 0xAA;
        private const int MAX_BLACK_OPACITY_FOR_VISIBLE = MAX_BLACK_OPACITY - 0x11;
        private const int VISUAL_RANGE = 10; // TODO proper visual range

        private readonly FOVComp FOVComp;
        private readonly BoardComp BoardComp;

        public FOVRenderer(UxContext uxContext, IRenderizable game)
            : base(uxContext)
        {
            BoardComp = game
                .Entities
                .GetWithComponent<BoardComp>()
                .Single()
                .GetComponent<BoardComp>();

            FOVComp = game
                .Entities
                .GetWithComponent<FOVComp>()
                .Single()
                .GetComponent<FOVComp>();
        }

        protected override void Render(IEntity entity, float interpolation)
        {
            for (var x = 0; x < BoardComp.BoardSize.Width; x++)
                for (var y = 0; y < BoardComp.BoardSize.Height; y++)
                    RenderTileFOV(new Point(x, y), FOVComp.FOVTiles[x, y]);

            SDL_SetRenderDrawColor(UxContext.WRenderer, 0, 0, 0, 0);
        }

        private void RenderTileFOV(Point point, TileFOVInfo tileFOVInfo)
        {
            if (!tileFOVInfo.Hidden && !tileFOVInfo.VisibleByPlayer)
                return;

            byte alpha;
            if (tileFOVInfo.Hidden)
                alpha = MAX_BLACK_OPACITY;
            else
                alpha = (byte)(
                    MAX_BLACK_OPACITY_FOR_VISIBLE
                    * (tileFOVInfo.DistanceFromPlayer / (BoardComp.TILE_SIZE * VISUAL_RANGE))
                );

            var screenPosition = GetScreenPosition(point.Multiply(BoardComp.TILE_SIZE));

            var tRect = new SDL_Rect()
            {
                x = screenPosition.X,
                y = screenPosition.Y,
                w = BoardComp.TILE_SIZE,
                h = BoardComp.TILE_SIZE
            };

            SDL_SetRenderDrawColor(UxContext.WRenderer, 0, 0, 0, alpha);
            SDL_RenderFillRect(UxContext.WRenderer, ref tRect);
        }
    }
}
