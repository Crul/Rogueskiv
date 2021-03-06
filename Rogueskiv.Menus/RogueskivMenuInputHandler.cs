﻿using Seedwork.Engine;
using Seedwork.Ux;
using System.Collections.Generic;
using static SDL2.SDL;

namespace Rogueskiv.Menus
{
    public class RogueskivMenuInputHandler : InputHandler<RogueskivMenu>
    {
        private readonly RogueskivMenu Game;

        private readonly List<SDL_Keycode> RepeatAllowedKeys = new List<SDL_Keycode>
        {
            SDL_Keycode.SDLK_LEFT, SDL_Keycode.SDLK_RIGHT,
            SDL_Keycode.SDLK_a, SDL_Keycode.SDLK_d,
            SDL_Keycode.SDLK_KP_4, SDL_Keycode.SDLK_KP_6,
        };

        public RogueskivMenuInputHandler(
            UxContext uxContext, RogueskivMenu game, IGameRenderer gameRenderer
        )
            : base(
                uxContext,
                game,
                gameRenderer,
                controlsByKeys: new Dictionary<int, int>
                {
                    { (int)SDL_Keycode.SDLK_ESCAPE,   (int)Controls.QUIT },
                    { (int)SDL_Keycode.SDLK_q,        (int)Controls.QUIT },

                    { (int)SDL_Keycode.SDLK_UP,       (int)Controls.UP },
                    { (int)SDL_Keycode.SDLK_DOWN,     (int)Controls.DOWN },
                    { (int)SDL_Keycode.SDLK_LEFT,     (int)Controls.LEFT },
                    { (int)SDL_Keycode.SDLK_RIGHT,    (int)Controls.RIGHT },

                    { (int)SDL_Keycode.SDLK_w,        (int)Controls.UP },
                    { (int)SDL_Keycode.SDLK_s,        (int)Controls.DOWN },
                    { (int)SDL_Keycode.SDLK_a,        (int)Controls.LEFT },
                    { (int)SDL_Keycode.SDLK_d,        (int)Controls.RIGHT },

                    { (int)SDL_Keycode.SDLK_KP_8,     (int)Controls.UP },
                    { (int)SDL_Keycode.SDLK_KP_2,     (int)Controls.DOWN },
                    { (int)SDL_Keycode.SDLK_KP_4,     (int)Controls.LEFT },
                    { (int)SDL_Keycode.SDLK_KP_6,     (int)Controls.RIGHT },

                    { (int)SDL_Keycode.SDLK_RETURN,   (int)Controls.ENTER },
                    { (int)SDL_Keycode.SDLK_RETURN2,  (int)Controls.ENTER },
                    { (int)SDL_Keycode.SDLK_KP_ENTER, (int)Controls.ENTER },
                    { (int)SDL_Keycode.SDLK_BACKSPACE,(int)Controls.BACKSPACE },
                    { (int)SDL_Keycode.SDLK_m,        (int)Controls.TOGGLE_MUSIC },
                },
                closeWindowControl: (int)Controls.CLOSE_WINDOW,
                toggleMusicControl: (int)Controls.TOGGLE_MUSIC,
                allowRepeats: true
            )
        {
            Game = game;
            ControlStates.Add((int)Controls.COPY, false);
            ControlStates.Add((int)Controls.PASTE, false);
        }

        public override void ProcessEvents()
        {
            base.ProcessEvents();
            Reset();
        }

        protected override void OnKeyEvent(SDL_Keycode key, bool pressed, bool isRepeat)
        {
            if (!isRepeat || RepeatAllowedKeys.Contains(key))
                base.OnKeyEvent(key, pressed, isRepeat);

            ControlStates[(int)Controls.COPY] = !isRepeat && (key == SDL_Keycode.SDLK_c && IsControlKeyPressed());
            ControlStates[(int)Controls.PASTE] = !isRepeat && (key == SDL_Keycode.SDLK_v && IsControlKeyPressed());
        }

        protected override void OnTextInput(string text)
        {
            var firstChar = text.ToUpper()[0];
            var isCopyOrPaste = IsControlKeyPressed() && (firstChar == 'C' || firstChar == 'V');
            if (isCopyOrPaste)
                return;

            Game.OnTextInput(text);
        }

        private static bool IsControlKeyPressed() =>
            (SDL_GetModState() & (SDL_Keymod.KMOD_CTRL | SDL_Keymod.KMOD_LCTRL)) != 0;
    }
}
