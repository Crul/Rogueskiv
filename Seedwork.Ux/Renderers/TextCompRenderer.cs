using Seedwork.Core.Components;
using Seedwork.Core.Entities;
using System;
using System.Drawing;
using static SDL2.SDL;

namespace Seedwork.Ux.Renderers
{
    public abstract class TextCompRenderer<T> : CompRenderer<T>
        where T : IComponent
    {
        protected readonly TextRenderer TextRenderer;

        protected TextCompRenderer(UxContext uxContext, IntPtr font)
            : base(uxContext) => TextRenderer = new TextRenderer(uxContext, font);

        protected override void Render(IEntity entity, T comp, float interpolation)
        {
            var text = GetText(comp);
            var textColor = GetColor(comp);
            var position = GetPosition(comp);
            var aligment = GetAligment();
            TextRenderer.Render(text, textColor, position, aligment, RenderBgr);
        }

        protected abstract string GetText(T component);
        protected abstract SDL_Color GetColor(T component);
        protected abstract Point GetPosition(T component);
        protected virtual TextAlign GetAligment() => TextAlign.CENTER;

        protected virtual void RenderBgr(Point position) { }

        protected override void Dispose(bool cleanManagedResources)
        {
            base.Dispose(cleanManagedResources);
            if (cleanManagedResources)
                TextRenderer.Dispose();
        }
    }
}
