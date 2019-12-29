using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Walls;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Crosscutting;
using Seedwork.Ux;
using Seedwork.Ux.SpriteProviders;
using System;
using System.Drawing;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class WallRenderer : FixedPositionRenderer<IWallComp>
    {
        public WallRenderer(UxContext uxContext, IRenderizable game, IntPtr boardTexture)
            : base(
                  uxContext,
                  game,
                  new SingleSpriteProvider<IWallComp>(
                      boardTexture,
                      new SDL_Rect()
                      {
                          x = 0,
                          y = BoardComp.TILE_SIZE,
                          w = BoardComp.TILE_SIZE,
                          h = BoardComp.TILE_SIZE
                      }
                  )
            )
        { }

        protected override void Render(
            ISpriteProvider<IWallComp> spriteProvider,
            IEntity entity,
            IWallComp wallComp,
            Point screenPosition
        )
        {
            var texture = spriteProvider.GetTexture(wallComp);
            var textureRect = spriteProvider.GetTextureRect(wallComp);
            var outputRect = spriteProvider.GetOutputRect(screenPosition);

            wallComp.GetTiles().ForEach(tilePos =>  // TODO ugly division inside GetTiles()
            {
                var tilePosition = tilePos.Multiply(BoardComp.TILE_SIZE).ToPoint();  // TODO ugly multiply
                outputRect.x = tilePosition.X;
                outputRect.y = tilePosition.Y;
                SDL_RenderCopy(UxContext.WRenderer, texture, ref textureRect, ref outputRect);
            });
        }
    }
}
