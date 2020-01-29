using Rogueskiv.Core.Components.Board;
using Rogueskiv.Core.Components.Walls;
using Rogueskiv.Ux.SoriteProviders;
using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Crosscutting;
using Seedwork.Ux;
using Seedwork.Ux.SpriteProviders;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static SDL2.SDL;

namespace Rogueskiv.Ux.Renderers
{
    class WallRenderer : FixedPositionRenderer<IWallComp>
    {
        private readonly List<(Point, WallSpriteProvider.SubTile)> Rendered;

        public WallRenderer(UxContext uxContext, IRenderizable game, IntPtr boardTexture)
            : base(
                  uxContext,
                  game,
                  new WallSpriteProvider(
                      boardTexture,
                      game.Entities.GetSingleComponent<BoardComp>()
                  )
            ) =>
            Rendered = new List<(Point, WallSpriteProvider.SubTile)>();

        public override void Render(List<IEntity> entities, float interpolation)
        {
            Rendered.Clear();
            base.Render(entities, interpolation);
        }

        protected override void Render(
            ISpriteProvider<IWallComp> spriteProvider,
            IEntity entity,
            IWallComp wallComp,
            Point screenPosition
        )
        {
            var texture = spriteProvider.GetTexture(wallComp);
            var wallSpriteProvider = (WallSpriteProvider)spriteProvider;

            wallComp.GetTiles()
                .SelectMany(tilePos =>
                    WallSpriteProvider
                        .GetSubTiles(tilePos.Multiply(2).ToPoint())
                        .Select(subTileInfo => (
                            tilePos,
                            subTileInfo.subTile,
                            subTileInfo.subTilePos
                        ))
                )
                .ToList()
                .ForEach(subTileInfo =>
                {
                    var outputRect = wallSpriteProvider.GetOutputRect(subTileInfo.subTile, subTileInfo.subTilePos);
                    if (Rendered.Contains((subTileInfo.tilePos, subTileInfo.subTile)))
                        return;

                    var textureRect = wallSpriteProvider.GetTextureRect(subTileInfo.subTile, subTileInfo.tilePos);

                    SDL_RenderCopy(UxContext.WRenderer, texture, ref textureRect, ref outputRect);
                    Rendered.Add((subTileInfo.tilePos, subTileInfo.subTile));
                });
        }
    }
}
