using Rogueskiv.Core.Components.Board;
using Seedwork.Ux.SpriteProviders;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static SDL2.SDL;

namespace Rogueskiv.Ux.SoriteProviders
{
    class TileSpriteProvider : SpriteProvider<TileComp>
    {

        private readonly IntPtr Texture;
        private readonly Size OutputSize = new Size(BoardComp.TILE_SIZE, BoardComp.TILE_SIZE);

        private readonly IDictionary<TileWallFlags, SDL_Rect> TextureRects =
            new List<(TileWallFlags wallFlags, (int tileX, int tileY) spritePosition)>
            {
                ( TileWallFlags.None,  (tileX: 3, tileY: 0 ) ),

                ( TileWallFlags.Up,    (tileX: 1, tileY: 2 ) ),
                ( TileWallFlags.Down,  (tileX: 1, tileY: 0 ) ),
                ( TileWallFlags.Left,  (tileX: 2, tileY: 1 ) ),
                ( TileWallFlags.Right, (tileX: 0, tileY: 1 ) ),

                ( TileWallFlags.Up | TileWallFlags.Left,    (tileX: 2, tileY: 2 ) ),
                ( TileWallFlags.Up | TileWallFlags.Right,   (tileX: 0, tileY: 2 ) ),
                ( TileWallFlags.Down | TileWallFlags.Left,  (tileX: 2, tileY: 0 ) ),
                ( TileWallFlags.Down | TileWallFlags.Right, (tileX: 0, tileY: 0 ) ),

                ( TileWallFlags.CornerUpLeft,    (tileX: 3, tileY: 1 ) ),
                ( TileWallFlags.CornerUpRight,   (tileX: 4, tileY: 1 ) ),
                ( TileWallFlags.CornerDownLeft,  (tileX: 3, tileY: 2 ) ),
                ( TileWallFlags.CornerDownRight, (tileX: 4, tileY: 2 ) ),
            }
            .ToDictionary(
                elem => elem.wallFlags,
                elem => new SDL_Rect()
                {
                    x = elem.spritePosition.tileX * BoardComp.TILE_SIZE,
                    y = elem.spritePosition.tileY * BoardComp.TILE_SIZE,
                    w = BoardComp.TILE_SIZE,
                    h = BoardComp.TILE_SIZE
                }
            );

        public TileSpriteProvider(IntPtr texture) =>
            Texture = texture;

        public override IntPtr GetTexture(TileComp comp) => Texture;

        public override SDL_Rect GetTextureRect(TileComp comp) => TextureRects[comp.WallFlags];

        public override SDL_Rect GetOutputRect(Point position)
            => GetOutputRect(position, OutputSize);
    }
}
