using Rogueskiv.Core.Components.Position;
using SDL2;
using Seedwork.Core.Entities;
using Seedwork.Ux;
using Seedwork.Ux.Renderers;
using System;
using System.Drawing;

namespace Rogueskiv.Ux.Renderers
{
    class PositionRenderer<T> : SpriteRenderer<T>
        where T : IPositionComp
    {
        public PositionRenderer(
            UxContext uxContext,
            string imgPath,
            SDL.SDL_Rect textureRect,
            Tuple<int, int> outputSize = null
        ) : base(uxContext, imgPath, textureRect, outputSize)
        { }

        public PositionRenderer(
            UxContext uxContext,
            IntPtr texture,
            SDL.SDL_Rect textureRect,
            Tuple<int, int> outputSize = null
        ) : base(uxContext, texture, textureRect, outputSize)
        { }

        protected override void Render(IEntity entity, float interpolation)
        {
            if (!entity.HasComponent<T>())
                return;

            var positionComp = entity.GetComponent<T>();
            if (!positionComp.Visible)
                return;

            var position = GetXY(entity, positionComp, interpolation);

            Render(position);
        }

        protected virtual PointF GetXY
            (IEntity entity, T positionComp, float interpolation) =>
            positionComp.Position;
    }
}
