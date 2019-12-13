using Rogueskiv.Core;
using Rogueskiv.Core.Components;
using Rogueskiv.Core.Entities;
using Rogueskiv.Ux.Renderers;
using SDL2;
using System.Collections.Generic;
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
            Renderers[typeof(PlayerComp)] = new PlayerRenderer(uxContext);
        }

        protected override void RenderGame(float interpolation)
        {
            var fillRects = new List<SDL.SDL_Rect>() {
                new SDL.SDL_Rect() { x = 60, y = 60, h = 120, w = 210 },
                new SDL.SDL_Rect() { x = 120, y = 180, h = 180, w = 90 },
                new SDL.SDL_Rect() { x = 60, y = 360, h = 120, w = 210 },
                new SDL.SDL_Rect() { x = 270, y = 90, h = 30, w = 300 },
                new SDL.SDL_Rect() { x = 540, y = 120, h = 30, w = 30 },
                new SDL.SDL_Rect() { x = 300, y = 150, h = 180, w = 270 },
                new SDL.SDL_Rect() { x = 300, y = 360, h = 90, w = 90 },
                new SDL.SDL_Rect() { x = 330, y = 330, h = 30, w = 30 },
                new SDL.SDL_Rect() { x = 270, y = 390, h = 30, w = 30 },
            };
            SDL.SDL_SetRenderDrawColor(WRenderer, 0x66, 0xFF, 0xFF, 0x66);
            fillRects.ForEach(fillRect => SDL.SDL_RenderFillRect(WRenderer, ref fillRect));
            SDL.SDL_SetRenderDrawColor(WRenderer, 255, 255, 255, 255);

            Renderers.ToList()
                .ForEach(r =>
                    r.Value.Render(Game.Entities.GetWithComponent(r.Key), interpolation)
                );
        }

    }
}
