using Rogueskiv.Core.Components.Position;
using SDL2;
using Seedwork.Core.Entities;
using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using System;

namespace Rogueskiv.Ux.Renderers
{
    class PositionRenderer<T> : SpriteRenderer<T>
        where T : PositionComp
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
            if (!entity.HasComponent<T>())
                return;

            var positionComp = entity.GetComponent<T>();
            if (!positionComp.Visible)
                return;

            var (x, y) = GetXY(entity, positionComp, interpolation);

            Render(x, y);
        }

        protected virtual (float x, float y) GetXY
            (IEntity entity, T positionComp, float interpolation) =>
            (positionComp.X, positionComp.Y);
    }
}
