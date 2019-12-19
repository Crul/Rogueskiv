using Rogueskiv.Core.Components;
using Rogueskiv.Core.Components.Position;
using SDL2;
using Seedwork.Core.Entities;
using Seedwork.Ux;
using System;

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

        protected override (float x, float y) GetXY
            (IEntity entity, T positionComp, float interpolation) =>
            Interpolate(entity, positionComp, interpolation);

        private (float x, float y) Interpolate
            (IEntity entity, T positionComp, float interpolation)
        {
            // TODO wall bounces ?
            var movementComp = entity.GetComponent<MovementComp>();
            return (
                positionComp.X + (movementComp.SpeedX * interpolation),
                positionComp.Y + (movementComp.SpeedY * interpolation)
            );
        }
    }
}
