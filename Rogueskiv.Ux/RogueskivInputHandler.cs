﻿using Rogueskiv.Core;
using Seedwork.Ux;
using System.Collections.Generic;
using static SDL2.SDL;

namespace Rogueskiv.Ux
{
    public class RogueskivInputHandler : InputHandler<RogueskivGame>
    {
        private const int QUIT_KEY = (int)SDL_Keycode.SDLK_q;

        public RogueskivInputHandler(RogueskivGame game) : base(
            game,
            keyControls: new Dictionary<int, int>
            {
                { (int)QUIT_KEY,               (int)Controls.QUIT },
                { (int)SDL_Keycode.SDLK_UP,    (int)Controls.UP },
                { (int)SDL_Keycode.SDLK_DOWN,  (int)Controls.DOWN },
                { (int)SDL_Keycode.SDLK_LEFT,  (int)Controls.LEFT },
                { (int)SDL_Keycode.SDLK_RIGHT, (int)Controls.RIGHT },
            },
            quitKey: QUIT_KEY
        )
        { }
    }
}