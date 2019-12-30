using Seedwork.Engine;
using Seedwork.Ux;
using System.Collections.Generic;
using static SDL2.SDL;

namespace Rogueskiv.Menus
{
    public class RogueskivMenuInputHandler : InputHandler<RogueskivMenu>
    {
        private const int QUIT_KEY = (int)SDL_Keycode.SDLK_ESCAPE;

        public RogueskivMenuInputHandler(
            UxContext uxContext, RogueskivMenu game, IGameRenderer gameRenderer
        )
            : base(
                uxContext,
                game,
                gameRenderer,
                keyControls: new Dictionary<int, int>
                {
                    { (int)QUIT_KEY,                  (int)Controls.QUIT },
                    { (int)SDL_Keycode.SDLK_q,        (int)Controls.QUIT },
                    { (int)SDL_Keycode.SDLK_UP,       (int)Controls.UP },
                    { (int)SDL_Keycode.SDLK_DOWN,     (int)Controls.DOWN },
                    { (int)SDL_Keycode.SDLK_RETURN,   (int)Controls.ENTER },
                    { (int)SDL_Keycode.SDLK_KP_ENTER, (int)Controls.ENTER },
                },
                quitKey: QUIT_KEY
            )
        { }
    }

}
