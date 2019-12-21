using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Position;
using SDL2;
using Seedwork.Core.Entities;
using Seedwork.Ux;
using System;
using System.Drawing;

namespace Rogueskiv.Ux.Renderers
{
    class InterpolatedPositionRenderer<T> : PositionRenderer<T>
        where T : IPositionComp
    {
        public InterpolatedPositionRenderer(
            UxContext uxContext,
            string imgPath,
            SDL.SDL_Rect textureRect,
            Tuple<int, int> outputSize = null
        ) : base(uxContext, imgPath, textureRect, outputSize)
        { }

        protected override PointF GetXY
            (IEntity entity, T positionComp, float interpolation) =>
            Interpolate(entity, positionComp, interpolation);

        private PointF Interpolate
            (IEntity entity, T positionComp, float interpolation)
        {
            // TODO wall bounces ?
            var movementComp = entity.GetComponent<MovementComp>();
            return new PointF(
                positionComp.Position.X + (movementComp.Speed.X * interpolation),
                positionComp.Position.Y + (movementComp.Speed.Y * interpolation)
            );
        }
    }
}
