using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Position;
using Seedwork.Core;
using Seedwork.Crosscutting;
using Seedwork.Ux;
using Seedwork.Ux.SpriteProviders;
using System.Drawing;

namespace Rogueskiv.Ux.Renderers
{
    class FixedPositionRenderer<T> : PositionRenderer<T>
        where T : IPositionComp
    {
        public FixedPositionRenderer(
            UxContext uxContext,
            IRenderizable game,
            ISpriteProvider<T> spriteProvider
        ) : base(uxContext, game, spriteProvider)
        { }

        protected override Point GetScreenPosition(PointF position) =>
            position.Add(BoardComp.TILE_SIZE / 2).ToPoint();
    }
}
