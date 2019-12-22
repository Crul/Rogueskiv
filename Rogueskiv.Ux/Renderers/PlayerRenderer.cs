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
        private const int CAMERA_MOVEMENT_FRICTION = 20;

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

            var lastMovmement = positionComp
                .Position
                .Substract(lastPositionComp.Position)
                .ToPoint();

            BgrTextureRect.x = Maths.Modulo(
                BgrTextureRect.x - lastMovmement.X,
                NonRepeatingBgrTextureSize
            );

            BgrTextureRect.y = Maths.Modulo(
                BgrTextureRect.y - lastMovmement.X,
                NonRepeatingBgrTextureSize
            );

            Render(position);
        }

        protected override void Render(PointF position)
        {
            SetUxCenter(UxContext, position, CAMERA_MOVEMENT_FRICTION);

            var screenPosition = GetScreenPosition(position);

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
                    x = screenPosition.X + maskRect.X - BgrOutputSize.Item1 / 2,
                    y = screenPosition.Y + maskRect.Y - BgrOutputSize.Item2 / 2,
                    w = maskRect.Width,
                    h = maskRect.Height
                };

                SDL_RenderCopy(UxContext.WRenderer, BgrTexture, ref bgrTextureRect, ref outRect);
            });

            base.Render(position);
        }

        public static void SetUxCenter(
            UxContext uxContext, PointF playerPosition, int friction = 1
        )
        {
            var targetCenter = uxContext
                .ScreenSize.ToPoint().Divide(2)
                .Substract(playerPosition.ToPoint());

            var cameraMovement = targetCenter
                .Substract(uxContext.Center)
                .Divide(friction);

            uxContext.Center = uxContext.Center.Add(cameraMovement);
        }

        protected override void Dispose(bool cleanManagedResources)
        {
            base.Dispose(cleanManagedResources);
            SDL_DestroyTexture(BgrTexture);
        }
    }
}
