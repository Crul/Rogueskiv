using SDL2;
using Seedwork.Core.Components;
using Seedwork.Core.Entities;
using System;
using System.Collections.Generic;

namespace Seedwork.Ux.Renderers
{
    public abstract class ItemRenderer<T> : IItemRenderer
        where T : IComponent
    {
        protected readonly UxContext UxContext;
        protected readonly Tuple<int, int> OutputSize;
        protected readonly IntPtr Texture;
        protected SDL.SDL_Rect TextureRect;

        public ItemRenderer(
            UxContext uxContext,
            string imgPath,
            SDL.SDL_Rect textureRect,
            Tuple<int, int> outputSize = null
        )
        {
            UxContext = uxContext;
            Texture = SDL_image.IMG_LoadTexture(UxContext.WRenderer, imgPath);
            TextureRect = textureRect;
            OutputSize = outputSize ?? new Tuple<int, int>(textureRect.w, textureRect.h);
        }

        public void Render(List<IEntity> entities, float interpolation) =>
            entities.ForEach(e => RenderIfComponent(e, interpolation));

        private void RenderIfComponent(IEntity entity, float interpolation)
        {
            if (entity.HasComponent<T>())
                Render(entity, interpolation);
        }

        protected abstract void Render(IEntity entity, float interpolation);

        protected virtual void Render(double posX, double posY)
        {
            var x = GetPositionComponent(posX, UxContext.CenterX);
            var y = GetPositionComponent(posY, UxContext.CenterY);

            var tRect = new SDL.SDL_Rect()
            {
                x = x - OutputSize.Item1 / 2,
                y = y - OutputSize.Item2 / 2,
                w = OutputSize.Item1,
                h = OutputSize.Item2
            };

            SDL.SDL_RenderCopy(UxContext.WRenderer, Texture, ref TextureRect, ref tRect);
        }

        protected static int GetPositionComponent(double positionComponent, int windowCenter) =>
            (int)(positionComponent * UxContext.Zoom) + windowCenter;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool cleanManagedResources) =>
            SDL.SDL_DestroyTexture(Texture);
    }
}
