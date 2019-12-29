using Rogueskiv.Core.Components.Position;
using Rogueskiv.Ux.SoriteProviders;
using Seedwork.Core.Entities;
using Seedwork.Crosscutting;
using Seedwork.Ux;
using Seedwork.Ux.SpriteProviders;
using System.Drawing;
using System.IO;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class PlayerRenderer : InterpolatedPositionRenderer<CurrentPositionComp>
    {
        public const int CAMERA_MOVEMENT_FRICTION = 20;

        private readonly PlayerAnimationProvider AnimationProvider;

        public PlayerRenderer(UxContext uxContext)
            : base(
                uxContext,
                new SingleSpriteProvider<CurrentPositionComp>(
                    uxContext,
                    Path.Combine("imgs", "player.png"),
                    new SDL_Rect { x = 0, y = 0, w = 36, h = 36 },
                    (36, 36)
                )
            ) =>
            AnimationProvider = new PlayerAnimationProvider(uxContext);

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

        protected override void Dispose(bool cleanManagedResources)
        {
            base.Dispose(cleanManagedResources);
            AnimationProvider.Dispose();
        }
    }
}
