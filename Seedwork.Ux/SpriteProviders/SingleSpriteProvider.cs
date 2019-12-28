using SDL2;
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

        private readonly bool ShouldDestroyTexture;

        public SingleSpriteProvider(
            UxContext uxContext,
            string texturePath,
            SDL_Rect? textureRect = null,
            (int width, int height)? outputSize = null
        ) : this(textureRect, outputSize)
        {
            Texture = SDL_image.IMG_LoadTexture(uxContext.WRenderer, texturePath);
            ShouldDestroyTexture = true;
        }

        public SingleSpriteProvider(
            IntPtr texture,
            SDL_Rect textureRect,
            (int width, int height)? outputSize = null
        ) : this(textureRect, outputSize)
        {
            Texture = texture;
            ShouldDestroyTexture = false;
        }

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

        public override SDL_Rect GetOutputRect(Point position)
            => GetOutputRect(position, OutputSize);

        protected override void Dispose(bool cleanManagedResources)
        {
            if (cleanManagedResources && ShouldDestroyTexture)
                SDL_DestroyTexture(Texture);
        }
    }
}
