using Rogueskiv.Core.Components.Position;
using SDL2;
using Seedwork.Core.Entities;
using Seedwork.Crosscutting;
using Seedwork.Ux;
using Seedwork.Ux.SpriteProviders;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using static SDL2.SDL;

namespace Rogueskiv.Ux.SoriteProviders
{
    class PlayerAnimationProvider : SpriteProvider<CurrentPositionComp>
    {
        private readonly IntPtr Texture;
        private SDL_Rect TextureRect;
        private SDL_Rect TextureRectMasked;
        private SDL_Rect OutputRect;
        private Size OutputSize;
        private readonly int NonRepeatingTextureSize;
        private readonly List<Rectangle> MaskTexture;

        public PlayerAnimationProvider(UxContext uxContext, int playerRadius)
        {
            Texture = SDL_image.IMG_LoadTexture(
                uxContext.WRenderer,
                Path.Combine("imgs", "player-texture.png")
            );
            var playerDiameter = playerRadius * 2;
            MaskTexture = Masks.GetCircleMask(playerRadius);
            TextureRect = new SDL_Rect { x = 0, y = 0, w = playerDiameter, h = playerDiameter };
            OutputSize = new Size(TextureRect.w, TextureRect.h);
            NonRepeatingTextureSize = 64 - TextureRect.w;
        }

        public void Render(
            IEntity entity,
            CurrentPositionComp currentPositionComp,
            Point screenPosition,
            Action<ISpriteProvider<CurrentPositionComp>> render)
        {
            var lastPositionComp = entity.GetComponent<LastPositionComp>();
            var lastMovement = currentPositionComp
                .Position
                .Substract(lastPositionComp.Position)
                .ToPoint();

            MoveTextureRect(lastMovement);

            MaskTexture.ForEach(maskRect =>
            {
                SetMask(maskRect, screenPosition);
                render(this);
            });
        }

        private void MoveTextureRect(Point lastMovmement)
        {
            TextureRect.x = Maths.Modulo(
                TextureRect.x - lastMovmement.X,
                NonRepeatingTextureSize
            );

            TextureRect.y = Maths.Modulo(
                TextureRect.y - lastMovmement.Y,
                NonRepeatingTextureSize
            );
        }

        private void SetMask(Rectangle maskRect, Point screenPosition)
        {
            TextureRectMasked = new SDL_Rect()
            {
                x = TextureRect.x + maskRect.X,
                y = TextureRect.y + maskRect.Y,
                w = maskRect.Width,
                h = maskRect.Height
            };

            OutputRect = new SDL_Rect()
            {
                x = screenPosition.X + maskRect.X - OutputSize.Width / 2,
                y = screenPosition.Y + maskRect.Y - OutputSize.Height / 2,
                w = maskRect.Width,
                h = maskRect.Height
            };
        }

        public override IntPtr GetTexture(CurrentPositionComp comp) =>
            Texture;

        public override SDL_Rect GetTextureRect(CurrentPositionComp comp, Point screenPosition) =>
            TextureRectMasked;

        public override SDL_Rect GetOutputRect(CurrentPositionComp comp, Point position) =>
            OutputRect;

        protected override void Dispose(bool cleanManagedResources)
        {
            base.Dispose(cleanManagedResources);
            SDL_DestroyTexture(Texture);
        }
    }
}
