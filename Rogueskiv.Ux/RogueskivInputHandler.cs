using Rogueskiv.Core;
using Seedwork.Engine;
using Seedwork.Ux;
using System.Collections.Generic;
using static SDL2.SDL;

namespace Rogueskiv.Ux
{
    public class RogueskivInputHandler : InputHandler<RogueskivGame>
    {
        public RogueskivInputHandler(
            UxContext uxContext, RogueskivGame game, IGameRenderer gameRenderer
        )
            : base(
                uxContext,
                game,
                gameRenderer,
                controlsByKeys: new Dictionary<int, int>
                {
                    { (int)SDL_Keycode.SDLK_q,     (int)Controls.QUIT },
                    { (int)SDL_Keycode.SDLK_ESCAPE,(int)Controls.PAUSE },
                    { (int)SDL_Keycode.SDLK_UP,    (int)Controls.UP },
                    { (int)SDL_Keycode.SDLK_DOWN,  (int)Controls.DOWN },
                    { (int)SDL_Keycode.SDLK_LEFT,  (int)Controls.LEFT },
                    { (int)SDL_Keycode.SDLK_RIGHT, (int)Controls.RIGHT },
                    { (int)SDL_Keycode.SDLK_s,     (int)Controls.TOGGLE_SOUNDS },
                    { (int)SDL_Keycode.SDLK_m,     (int)Controls.TOGGLE_MUSIC },
                },
                closeWindowControl: (int)Controls.CLOSE_WINDOW,
                toggleMusicControl: (int)Controls.TOGGLE_MUSIC
            )
        { }
    }
}
