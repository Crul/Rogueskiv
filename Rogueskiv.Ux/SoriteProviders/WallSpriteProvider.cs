using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Walls;
using Seedwork.Crosscutting;
using Seedwork.Ux.SpriteProviders;
using System;
using System.Collections.Generic;
using System.Drawing;
using static SDL2.SDL;

namespace Rogueskiv.Ux.SoriteProviders
{
    class WallSpriteProvider : SingleSpriteProvider<IWallComp>
    {
        public const int SUB_TILE_SIZE = 40;
        private const int SUB_TILE_MARGIN = 4;

        public enum SubTile
        {
            LEFT_TOP,
            TOP_RIGHT,
            RIGHT_BOTTOM,
            BOTTOM_LEFT
        }

        private readonly BoardComp BoardComp;

        private readonly SDL_Rect EmptyTextureRect = new SDL_Rect() { x = 0, y = 64, w = 1, h = 1 };

        private readonly IDictionary<(SubTile subTile, bool wall1, bool wall2), SDL_Rect> TextureRects =
            new Dictionary<(SubTile subTile, bool wall1, bool wall2), SDL_Rect>
            {
                { (SubTile.LEFT_TOP, false, false), new SDL_Rect(){ x=0, y=64, w=SUB_TILE_SIZE/2, h=SUB_TILE_SIZE/2 } },
                { (SubTile.LEFT_TOP, true,  true),  new SDL_Rect(){ x=40, y=64, w=SUB_TILE_SIZE/2, h=SUB_TILE_SIZE/2 } },
                { (SubTile.LEFT_TOP, true,  false), new SDL_Rect(){ x=0, y=104, w=SUB_TILE_SIZE/2, h=SUB_TILE_SIZE/2 } },
                { (SubTile.LEFT_TOP, false, true),  new SDL_Rect(){ x=40, y=104, w=SUB_TILE_SIZE/2, h=SUB_TILE_SIZE/2 } },

                { (SubTile.TOP_RIGHT, false, false), new SDL_Rect(){ x=20, y=64, w=SUB_TILE_SIZE/2, h=SUB_TILE_SIZE/2 } },
                { (SubTile.TOP_RIGHT, true,  true),  new SDL_Rect(){ x=60, y=64, w=SUB_TILE_SIZE/2, h=SUB_TILE_SIZE/2 } },
                { (SubTile.TOP_RIGHT, true, false),  new SDL_Rect(){ x=60, y=104, w=SUB_TILE_SIZE/2, h=SUB_TILE_SIZE/2 } },
                { (SubTile.TOP_RIGHT, false, true),  new SDL_Rect(){ x=20, y=104, w=SUB_TILE_SIZE/2, h=SUB_TILE_SIZE/2 } },

                { (SubTile.RIGHT_BOTTOM, false, false), new SDL_Rect(){ x=20, y=84, w=SUB_TILE_SIZE/2, h=SUB_TILE_SIZE/2 } },
                { (SubTile.RIGHT_BOTTOM, true,  true),  new SDL_Rect(){ x=60, y=84, w=SUB_TILE_SIZE/2, h=SUB_TILE_SIZE/2 } },
                { (SubTile.RIGHT_BOTTOM, true, false),  new SDL_Rect(){ x=20, y=124, w=SUB_TILE_SIZE/2, h=SUB_TILE_SIZE/2 } },
                { (SubTile.RIGHT_BOTTOM, false, true),  new SDL_Rect(){ x=60, y=124, w=SUB_TILE_SIZE/2, h=SUB_TILE_SIZE/2 } },

                { (SubTile.BOTTOM_LEFT, false, false), new SDL_Rect(){ x=0, y=84, w=SUB_TILE_SIZE/2, h=SUB_TILE_SIZE/2 } },
                { (SubTile.BOTTOM_LEFT, true,  true),  new SDL_Rect(){ x=40, y=84, w=SUB_TILE_SIZE/2, h=SUB_TILE_SIZE/2 } },
                { (SubTile.BOTTOM_LEFT, true, false),  new SDL_Rect(){ x=40, y=124, w=SUB_TILE_SIZE/2, h=SUB_TILE_SIZE/2 } },
                { (SubTile.BOTTOM_LEFT, false, true),  new SDL_Rect(){ x=0, y=124, w=SUB_TILE_SIZE/2, h=SUB_TILE_SIZE/2 } },
            };

        public WallSpriteProvider(IntPtr texture, BoardComp boardComp)
            : base(texture, outputSize: (SUB_TILE_SIZE / 2, SUB_TILE_SIZE / 2)) =>
            BoardComp = boardComp;

        public override SDL_Rect GetTextureRect(IWallComp comp, Point screenPosition) =>
            throw new NotImplementedException();

        public SDL_Rect GetTextureRect(SubTile subTile, Point tilePos)
        {
            bool wall1, wall2;
            switch (subTile)
            {
                case SubTile.LEFT_TOP:
                    wall1 = IsWall(tilePos.Substract(x: 1));
                    wall2 = IsWall(tilePos.Substract(y: 1));
                    if (wall1 && wall2 && IsWall(tilePos.Substract(x: 1, y: 1)))
                        return EmptyTextureRect;

                    break;
                case SubTile.TOP_RIGHT:
                    wall1 = IsWall(tilePos.Substract(y: 1));
                    wall2 = IsWall(tilePos.Add(x: 1));
                    if (wall1 && wall2 && IsWall(tilePos.Add(x: 1, y: -1)))
                        return EmptyTextureRect;

                    break;
                case SubTile.RIGHT_BOTTOM:
                    wall1 = IsWall(tilePos.Add(x: 1));
                    wall2 = IsWall(tilePos.Add(y: 1));
                    if (wall1 && wall2 && IsWall(tilePos.Add(x: 1, y: 1)))
                        return EmptyTextureRect;

                    break;
                case SubTile.BOTTOM_LEFT:
                    wall1 = IsWall(tilePos.Add(y: 1));
                    wall2 = IsWall(tilePos.Substract(x: 1));
                    if (wall1 && wall2 && IsWall(tilePos.Add(x: -1, y: 1)))
                        return EmptyTextureRect;

                    break;
                default:
                    throw new NotImplementedException();
            }

            return TextureRects[(subTile, wall1, wall2)];
        }

        public override SDL_Rect GetOutputRect(Point screenPosition) =>
            throw new NotImplementedException();

        public SDL_Rect GetOutputRect(SubTile subtile, Point subTilePos)
        {
            var screenPos = subTilePos.Multiply(BoardComp.TILE_SIZE / 2).ToPoint();
            var outputRect = new SDL_Rect()
            {
                x = screenPos.X,
                y = screenPos.Y,
                w = OutputSize.Width,
                h = OutputSize.Height
            };
            switch (subtile)
            {
                case SubTile.LEFT_TOP:
                    outputRect.x -= SUB_TILE_MARGIN;
                    outputRect.y -= SUB_TILE_MARGIN;
                    break;
                case SubTile.TOP_RIGHT:
                    outputRect.y -= SUB_TILE_MARGIN;
                    break;
                case SubTile.BOTTOM_LEFT:
                    outputRect.x -= SUB_TILE_MARGIN;
                    break;
            }

            return outputRect;
        }

        private bool IsWall(Point tilePos) =>
            !BoardComp.TileIdByTilePos.ContainsKey(tilePos);

        public static List<(SubTile subTile, Point subTilePos)> GetSubTiles(Point subTilePos) =>
            new List<(SubTile, Point)>
            {
                (SubTile.LEFT_TOP, subTilePos),
                (SubTile.TOP_RIGHT, subTilePos.Add(x: 1)),
                (SubTile.BOTTOM_LEFT, subTilePos.Add(y: 1)),
                (SubTile.RIGHT_BOTTOM, subTilePos.Add(1)),
            };
    }
}
