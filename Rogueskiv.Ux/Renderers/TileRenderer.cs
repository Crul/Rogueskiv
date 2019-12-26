using Rogueskiv.Core.Components.Board;
using Rogueskiv.Ux.SoriteProviders;
using Seedwork.Core;
using Seedwork.Ux;
using System;

namespace Rogueskiv.Ux.Renderers
{
    class TileRenderer : PositionRenderer<TileComp>
    {
        public TileRenderer(UxContext uxContext, IRenderizable game, IntPtr boardTexture)
            : base(
                uxContext,
                game,
                new TileSpriteProvider(boardTexture)
            )
        { }
    }
}
