using Rogueskiv.Core.Components.Position;
using Seedwork.Core.Entities;
using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using Seedwork.Ux.SpriteProviders;
using System.Drawing;

namespace Rogueskiv.Ux.Renderers
{
    class PositionRenderer<T> : SpriteRenderer<T>
        where T : IPositionComp
    {
        public PositionRenderer(UxContext uxContext, ISpriteProvider<T> spriteProvider)
            : base(uxContext, spriteProvider) { }

        protected override PointF GetPosition(
            IEntity entity, T positionComp, float interpolation
        ) => positionComp.Position;

        protected override void Render(IEntity entity, T positionComp, float interpolation)
        {
            if (positionComp.Visible)
                base.Render(entity, positionComp, interpolation);
        }
    }
}
