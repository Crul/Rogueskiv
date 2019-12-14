﻿using Rogueskiv.Core;
using Rogueskiv.Core.Components;
using Rogueskiv.Core.Entities;
using Rogueskiv.Ux.Renderers;
using System.Linq;

namespace Rogueskiv.Ux
{
    public class RogueskivRenderer : Renderer
    {
        private readonly IRenderizable Game;

        public RogueskivRenderer(IRenderizable game) : base("Rogueskiv")
        {
            Game = game;
            var uxContext = new UxContext(WRenderer);
            // TODO cache rendered background
            Renderers[typeof(TileComp)] = new TileRenderer(uxContext);
            Renderers[typeof(PlayerComp)] = new PlayerRenderer(uxContext);
        }

        protected override void RenderGame(float interpolation) =>
            Renderers.ToList()
                .ForEach(r =>
                    r.Value.Render(Game.Entities.GetWithComponent(r.Key), interpolation)
                );
    }
}
