using Seedwork.Core.Components;
using System;
using System.Drawing;
using static SDL2.SDL;

namespace Seedwork.Ux.SpriteProviders
{
    public class SingleSpriteProvider<T> : SpriteProvider<T>
        where T : IComponent
    {
        protected Size OutputSize { get; }
        protected IntPtr Texture { get; }
        protected SDL_Rect TextureRect { get; }

        public SingleSpriteProvider(
            UxContext uxContext,
            string imageFile,
            SDL_Rect? textureRect = null,
            (int width, int height)? outputSize = null
        ) : this(textureRect, outputSize) =>
            Texture = uxContext.GetTexture(imageFile);

        public SingleSpriteProvider(
            IntPtr texture,
            SDL_Rect? textureRect = null,
            (int width, int height)? outputSize = null
        ) : this(textureRect, outputSize) =>
            Texture = texture;

        private SingleSpriteProvider(
            SDL_Rect? textureRect = null,
            (int width, int height)? outputSize = null
        )
        {
            if (textureRect.HasValue)
                TextureRect = textureRect.Value;

            if (outputSize.HasValue || textureRect.HasValue)
                OutputSize = new Size(
                    outputSize?.width ?? textureRect.Value.w,
                    outputSize?.height ?? textureRect.Value.h
                );
        }

        public override IntPtr GetTexture(T comp) => Texture;

        public override SDL_Rect GetTextureRect(T comp, Point screenPosition) => TextureRect;

        public override SDL_Rect GetOutputRect(T comp, Point screenPosition)
            => GetOutputRect(screenPosition, OutputSize);
    }
}
