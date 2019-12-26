using Rogueskiv.Core.Components;
using Rogueskiv.Ux.SpriteProviders;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Engine;
using Seedwork.Ux;
using Seedwork.Ux.SpriteProviders;
using System;
using System.Drawing;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class PickableRenderer<T> : PositionRenderer<T>
        where T : PickableComp
    {
        private readonly IGameRenderer GameRenderer;

        public PickableRenderer(
            IGameRenderer gameRenderer,
            UxContext uxContext,
            IRenderizable game,
            IntPtr texture,
            SDL_Rect textureRect
        )
            : base(
                uxContext,
                game,
                new PickableSpriteProvider<T>(texture, textureRect)
            ) =>
            GameRenderer = gameRenderer;

        protected override void Render(
            ISpriteProvider<T> spriteProvider,
            IEntity entity,
            T pickableComp,
            Point screenPosition
        )
        {
            if (pickableComp.PickingTime == 0)
                base.Render(spriteProvider, entity, pickableComp, screenPosition);
            else
            {
                var pickableSpriteProvider = (PickableSpriteProvider<T>)spriteProvider;
                pickableSpriteProvider.PickableItemComp = pickableComp;

                GameRenderer.AddRenderOnEnd(() =>
                    base.Render(pickableSpriteProvider, entity, pickableComp, screenPosition)
                );
            }
        }
    }
}
