using Rogueskiv.Core.Components.Board;
using Rogueskiv.Ux.SoriteProviders;
using Seedwork.Ux;
using System;

namespace Rogueskiv.Ux.Renderers
{
    class TileRenderer : PositionRenderer<TileComp>
    {
        public TileRenderer(UxContext uxContext, IntPtr boardTexture)
            : base(
                uxContext,
                new TileSpriteProvider(boardTexture)
            )
        { }
    }
}
