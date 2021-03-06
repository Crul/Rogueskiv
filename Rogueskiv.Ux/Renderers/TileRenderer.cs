﻿using Rogueskiv.Core.Components.Board;
using Rogueskiv.Ux.SoriteProviders;
using Seedwork.Core;
using Seedwork.Ux;
using System.Drawing;

namespace Rogueskiv.Ux.Renderers
{
    class TileRenderer : FixedPositionRenderer<TileComp>
    {
        public TileRenderer(UxContext uxContext, IRenderizable game)
            : base(
                uxContext,
                game,
                new TileSpriteProvider(
                    uxContext,
                    "floor.png",
                    new Size(1920, 1440)
                )
            )
        { }
    }
}
