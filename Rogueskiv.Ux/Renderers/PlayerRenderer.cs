﻿using Rogueskiv.Core.Components;
using Rogueskiv.Core.Entities;
using System.IO;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class PlayerRenderer : ItemRenderer
    {
        public PlayerRenderer(UxContext uxContext)
            : base(
                  uxContext,
                  Path.Combine("imgs", "player.png"),
                  new SDL_Rect { x = 0, y = 0, w = 48, h = 48 }
            )
        { }

        protected override void Render(IEntity entity, float interpolation)
        {
            if (!entity.HasComponent<PositionComp>())
                return;

            var positionComp = entity.GetComponent<PositionComp>();
            var (x, y) = Interpolate(entity, positionComp, interpolation);

            Render(x, y);
        }

        private (float, float) Interpolate(IEntity entity, PositionComp positionComp, float interpolation)
        {
            if (interpolation <= 0 || !entity.HasComponent<MovementComp>())
                return (positionComp.X, positionComp.Y);

            var movementComp = entity.GetComponent<MovementComp>();
            return (
                positionComp.X + (movementComp.SpeedX * interpolation),
                positionComp.Y + (movementComp.SpeedY * interpolation)
            );
        }
    }
}
