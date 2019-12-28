using Seedwork.Core.Components;
using Seedwork.Core.Entities;
using Seedwork.Ux.SpriteProviders;
using System.Drawing;
using static SDL2.SDL;

namespace Seedwork.Ux.Renderers
{
    public abstract class SpriteRenderer<T> : CompRenderer<T>
        where T : IComponent
    {
        private readonly ISpriteProvider<T> SpriteProvider;

        protected SpriteRenderer(UxContext uxContext, ISpriteProvider<T> spriteProvider)
            : base(uxContext) =>
            SpriteProvider = spriteProvider;

        protected override void Render(IEntity entity, T comp, float interpolation) =>
            Render(SpriteProvider, entity, comp, interpolation);

        protected virtual void Render(
            ISpriteProvider<T> spriteProvider,
            IEntity entity,
            T comp,
            float interpolation
        )
        {
            var position = GetPosition(entity, comp, interpolation);
            var screenPosition = GetScreenPosition(position);
            Render(spriteProvider, entity, comp, screenPosition);
        }

        protected virtual void Render(
            ISpriteProvider<T> spriteProvider,
            IEntity entity,
            T comp,
            Point screenPosition
        )
        {
            var texture = spriteProvider.GetTexture(comp);
            var textureRect = spriteProvider.GetTextureRect(comp);
            var outputRect = spriteProvider.GetOutputRect(screenPosition);

            SDL_RenderCopy(UxContext.WRenderer, texture, ref textureRect, ref outputRect);
        }

        protected abstract PointF GetPosition(IEntity entity, T comp, float interpolation);

        protected override void Dispose(bool cleanManagedResources)
        {
            if (cleanManagedResources)
                SpriteProvider.Dispose();
        }
    }
}
