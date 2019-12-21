using Rogueskiv.Core.Components.Position;
using SDL2;
using Seedwork.Core.Entities;
using Seedwork.Crosscutting;
using Seedwork.Ux;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class PlayerRenderer : InterpolatedPositionRenderer<CurrentPositionComp>
    {
        private readonly IntPtr BgrTexture;
        private SDL_Rect BgrTextureRect;
        private readonly Tuple<int, int> BgrOutputSize;
        private readonly int NonRepeatingBgrTextureSize;

        private readonly List<Rectangle> BgrMaskTexture = Masks
            .GetFromImage(Path.Combine("imgs", "player-texture-mask.png"));

        public PlayerRenderer(UxContext uxContext)
            : base(
                  uxContext,
                  Path.Combine("imgs", "player.png"),
                  new SDL_Rect { x = 0, y = 0, w = 36, h = 36 },
                  new Tuple<int, int>(36, 36)
            )
        {
            BgrTexture = SDL_image.IMG_LoadTexture(
                UxContext.WRenderer,
                Path.Combine("imgs", "player-texture.png")
            );
            BgrTextureRect = new SDL_Rect { x = 0, y = 0, w = 16, h = 16 };
            BgrOutputSize = new Tuple<int, int>(20, 20);
            NonRepeatingBgrTextureSize = 48;
        }

        protected override void Render(IEntity entity, float interpolation)
        {
            if (!entity.HasComponent<CurrentPositionComp>())
                return;

            var positionComp = entity.GetComponent<CurrentPositionComp>();
            if (!positionComp.Visible)
                return;

            var lastPositionComp = entity.GetComponent<LastPositionComp>();
            var position = GetXY(entity, positionComp, interpolation);

            BgrTextureRect.x = Maths.Modulo(
                BgrTextureRect.x - (int)(positionComp.Position.X - lastPositionComp.Position.X),
                NonRepeatingBgrTextureSize
            );

            BgrTextureRect.y = Maths.Modulo(
                BgrTextureRect.y - (int)(positionComp.Position.Y - lastPositionComp.Position.Y),
                NonRepeatingBgrTextureSize
            );

            Render(position);
        }

        protected override void Render(PointF position)
        {
            UxContext.CenterX = (int)((UxContext.ScreenWidth / 2) - position.X);
            UxContext.CenterY = (int)((UxContext.ScreenHeight / 2) - position.Y);

            var screenPosition = GetScreenPosition(position, UxContext);

            BgrMaskTexture.ForEach(maskRect =>
            {
                var bgrTextureRect = new SDL_Rect()
                {
                    x = BgrTextureRect.x + maskRect.X,
                    y = BgrTextureRect.y + maskRect.Y,
                    w = maskRect.Width,
                    h = maskRect.Height
                };

                var outRect = new SDL_Rect()
                {
                    x = (int)screenPosition.X + maskRect.X - BgrOutputSize.Item1 / 2,
                    y = (int)screenPosition.Y + maskRect.Y - BgrOutputSize.Item2 / 2,
                    w = maskRect.Width,
                    h = maskRect.Height
                };

                SDL_RenderCopy(UxContext.WRenderer, BgrTexture, ref bgrTextureRect, ref outRect);
            });

            base.Render(position);
        }

        protected override void Dispose(bool cleanManagedResources)
        {
            base.Dispose(cleanManagedResources);
            SDL_DestroyTexture(BgrTexture);
        }
    }
}
