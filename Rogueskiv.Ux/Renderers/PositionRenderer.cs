using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Position;
using Seedwork.Core;
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
        protected FOVComp FOVComp { get; }

        public PositionRenderer(
            UxContext uxContext, IRenderizable game, ISpriteProvider<T> spriteProvider
        )
            : base(uxContext, spriteProvider) =>
            FOVComp = game.Entities.GetSingleComponent<FOVComp>();

        protected override PointF GetPosition(
            IEntity entity, T positionComp, float interpolation
        ) => positionComp.Position;

        protected override void Render(IEntity entity, T positionComp, float interpolation)
        {
            if (FOVComp.IsVisible(positionComp))
                base.Render(entity, positionComp, interpolation);
        }
    }
}
