using Seedwork.Engine;
using Seedwork.Ux;
using System.Collections.Generic;
using static SDL2.SDL;

namespace Rogueskiv.Menus
{
    public class RogueskivMenuInputHandler : InputHandler<RogueskivMenu>
    {
        private const int QUIT_KEY = (int)SDL_Keycode.SDLK_ESCAPE;

        private readonly RogueskivMenu Game;

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
                    { (int)SDL_Keycode.SDLK_UP,       (int)Controls.UP },
                    { (int)SDL_Keycode.SDLK_DOWN,     (int)Controls.DOWN },
                    { (int)SDL_Keycode.SDLK_RETURN,   (int)Controls.ENTER },
                    { (int)SDL_Keycode.SDLK_KP_ENTER, (int)Controls.ENTER },
                    { (int)SDL_Keycode.SDLK_BACKSPACE,(int)Controls.BACKSPACE},
                },
                quitKey: QUIT_KEY
            )
        {
            Game = game;
            KeyPressStates.Add((int)Controls.COPY, false);
            KeyPressStates.Add((int)Controls.PASTE, false);
        }

        protected override void OnKeyEvent(SDL_Keycode key, bool pressed)
        {
            base.OnKeyEvent(key, pressed);

            KeyPressStates[(int)Controls.COPY] = key == SDL_Keycode.SDLK_c && IsControlKeyPressed();
            KeyPressStates[(int)Controls.PASTE] = key == SDL_Keycode.SDLK_v && IsControlKeyPressed();
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
