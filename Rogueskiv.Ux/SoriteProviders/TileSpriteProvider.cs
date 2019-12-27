using Rogueskiv.Core.Components.Board;
using Seedwork.Ux.SpriteProviders;
using System;
using static SDL2.SDL;

namespace Rogueskiv.Ux.SoriteProviders
{
    class TileSpriteProvider : SingleSpriteProvider<TileComp>
    {
        public TileSpriteProvider(IntPtr texture)
            : base(
                  texture,
                  new SDL_Rect()
                  {
                      x = 0,
                      y = 0,
                      w = BoardComp.TILE_SIZE,
                      h = BoardComp.TILE_SIZE
                  }
            )
        { }
    }
}
