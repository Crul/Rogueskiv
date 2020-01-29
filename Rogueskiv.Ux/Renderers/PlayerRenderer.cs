using Rogueskiv.Core.Components.Position;
using Rogueskiv.Ux.SoriteProviders;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Crosscutting;
using Seedwork.Ux;
using Seedwork.Ux.SpriteProviders;
using System.Drawing;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class PlayerRenderer : InterpolatedPositionRenderer<CurrentPositionComp>
    {
        private readonly PlayerAnimationProvider AnimationProvider;

        public PlayerRenderer(UxContext uxContext, IRenderizable game, int playerRadius)
            : base(
                uxContext,
                game,
                new SingleSpriteProvider<CurrentPositionComp>(
                    uxContext,
                    "player.png",
                    GetPlayerTextureRect(playerRadius)
                )
            ) =>
            AnimationProvider = new PlayerAnimationProvider(uxContext, playerRadius);

        protected override void Render(
            ISpriteProvider<CurrentPositionComp> spriteProvider,
            IEntity entity,
            CurrentPositionComp currentPositionComp,
            Point screenPosition
        )
        {
            AnimationProvider.Render(
                entity,
                currentPositionComp,
                screenPosition,
                spriteProv => base.Render(AnimationProvider, entity, currentPositionComp, screenPosition)
            );

            base.Render(spriteProvider, entity, currentPositionComp, screenPosition);
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

        private static SDL_Rect GetPlayerTextureRect(int playerRadius)
        {
            var spriteSize = (int)(3.6f * playerRadius);

            return new SDL_Rect { x = 0, y = 0, w = spriteSize, h = spriteSize };
        }

        protected override void Dispose(bool cleanManagedResources)
        {
            base.Dispose(cleanManagedResources);
            AnimationProvider.Dispose();
        }
    }
}
