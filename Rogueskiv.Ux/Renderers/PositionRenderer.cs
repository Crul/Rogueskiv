using Rogueskiv.Core.Components;
using Rogueskiv.Core.Entities;
using SDL2;
using System;

namespace Rogueskiv.Ux.Renderers
{
    class PositionRenderer : ItemRenderer<PositionComp>
    {
        public PositionRenderer(
            UxContext uxContext,
            string imgPath,
            SDL.SDL_Rect textureRect,
            Tuple<int, int> outputSize = null
        ) : base(uxContext, imgPath, textureRect, outputSize)
        { }

        protected override void Render(IEntity entity, float interpolation)
        {
            if (!entity.HasComponent<PositionComp>())
                return;

            var positionComp = entity.GetComponent<PositionComp>();
            var (x, y) = Interpolate(entity, positionComp, interpolation);

            Render(x, y);
        }

        protected virtual (float, float) Interpolate(
            IEntity entity,
            PositionComp positionComp,
            float interpolation
        )
        {
            // TODO wall bounces ?
            if (!entity.HasComponent<MovementComp>())
                return (positionComp.X, positionComp.Y);

            var movementComp = entity.GetComponent<MovementComp>();
            return (
                positionComp.X + (movementComp.SpeedX * interpolation),
                positionComp.Y + (movementComp.SpeedY * interpolation)
            );
        }
    }
}
