using Rogueskiv.Core.Components;
using Seedwork.Crosscutting;
using Seedwork.Ux;
using Seedwork.Ux.SpriteProviders;
using System;
using System.Drawing;
using static SDL2.SDL;

namespace Rogueskiv.Ux.SpriteProviders
{
    class PickableSpriteProvider<T> : SingleSpriteProvider<T>
        where T : PickableComp
    {
        private const float MAX_SIZE_FACTOR = 2;

        public T PickableItemComp { get; set; }

        public PickableSpriteProvider(
            UxContext uxContext,
            string texturePath,
            SDL_Rect textureRect,
            (int width, int height)? outputSize = null
        ) : base(uxContext, texturePath, textureRect, outputSize)
        { }

        public PickableSpriteProvider(
            IntPtr texture,
            SDL_Rect textureRect,
            (int width, int height)? outputSize = null
        ) : base(texture, textureRect, outputSize)
        { }

        public override SDL_Rect GetOutputRect(T pickableComp, Point position)
        {
            var outputSize = OutputSize;
            if (PickableItemComp?.IsBeingPicked ?? false)
            {
                var sizeFactor = 1 + (
                    (MAX_SIZE_FACTOR - 1)
                    * (pickableComp.MaxPickingTime - PickableItemComp.PickingTime)
                    / pickableComp.MaxPickingTime
                );

                outputSize = outputSize.Multiply(sizeFactor);
            }

            return GetOutputRect(position, outputSize);
        }
    }
}
