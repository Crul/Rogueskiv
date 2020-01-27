using Rogueskiv.Core.Components.Board;
using Seedwork.Crosscutting;
using Seedwork.Ux;
using Seedwork.Ux.SpriteProviders;
using System.Drawing;
using static SDL2.SDL;

namespace Rogueskiv.Ux.SoriteProviders
{
    class TileSpriteProvider : SingleSpriteProvider<TileComp>
    {
        private readonly Size TextureSize;

        public TileSpriteProvider(UxContext uxContext, string imagePath, Size textureSize)
            : base(
                  uxContext,
                  imagePath,
                  outputSize: (BoardComp.TILE_SIZE, BoardComp.TILE_SIZE)
            ) =>
            TextureSize = textureSize;

        public override SDL_Rect GetTextureRect(TileComp comp, Point screenPosition) => new SDL_Rect()
        {
            x = Maths.Modulo(screenPosition.X, TextureSize.Width),
            y = Maths.Modulo(screenPosition.Y, TextureSize.Height),
            w = BoardComp.TILE_SIZE,
            h = BoardComp.TILE_SIZE
        };
    }
}
