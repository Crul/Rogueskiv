using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Position;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Crosscutting;
using Seedwork.Ux;
using Seedwork.Ux.SpriteProviders;
using System.Drawing;

namespace Rogueskiv.Ux.Renderers
{
    class InterpolatedPositionRenderer<T> : PositionRenderer<T>
        where T : IPositionComp
    {
        public InterpolatedPositionRenderer(
            UxContext uxContext, IRenderizable game, ISpriteProvider<T> spriteProvider
        )
            : base(uxContext, game, spriteProvider) { }

        protected override PointF GetPosition(
            IEntity entity, T positionComp, float interpolation
        )
        {
            // TODO wall bounces ?
            var movementComp = entity.GetComponent<MovementComp>();

            return positionComp.Position.Add(movementComp.Speed.Multiply(interpolation));
        }
    }
}
