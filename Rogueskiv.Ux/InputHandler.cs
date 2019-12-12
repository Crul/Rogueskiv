using Rogueskiv.Core.Controls;
using Rogueskiv.Engine;
using System.Collections.Generic;
using System.Linq;
using static SDL2.SDL;

namespace Rogueskiv.Ux
{
    public class InputHandler<T> : IInputHandler
        where T : IControlable
    {
        private readonly T Game;

        private const SDL_Keycode QUIT_KEY = SDL_Keycode.SDLK_q;

        private readonly IDictionary<SDL_Keycode, Control> KeyControls =
            new Dictionary<SDL_Keycode, Control>
            {
                { QUIT_KEY,               Control.QUIT },
                { SDL_Keycode.SDLK_UP,    Control.UP },
                { SDL_Keycode.SDLK_DOWN,  Control.DOWN },
                { SDL_Keycode.SDLK_LEFT,  Control.LEFT },
                { SDL_Keycode.SDLK_RIGHT, Control.RIGHT },
            };

        private readonly IDictionary<SDL_Keycode, bool> KeyPressStates;

        public InputHandler(T game)
        {
            Game = game;
            KeyPressStates = KeyControls
                .ToDictionary(kc => kc.Key, _ => false);
        }

        public void ProcessEvents()
        {
            while (SDL_PollEvent(out SDL_Event ev) != 0)
                ProcessEvent(ev);

            Game.Controls = KeyPressStates
                .Where(keyState => keyState.Value)
                .Select(keyState => KeyControls[keyState.Key]);
        }

        private void ProcessEvent(SDL_Event ev)
        {
            switch (ev.type)
            {
                case SDL_EventType.SDL_QUIT:
                    KeyPressStates[QUIT_KEY] = true;
                    return;

                case SDL_EventType.SDL_KEYDOWN:
                    OnKeyEvent(ev.key.keysym.sym, true);
                    break;

                case SDL_EventType.SDL_KEYUP:
                    OnKeyEvent(ev.key.keysym.sym, false);
                    break;
            }
        }

        private void OnKeyEvent(SDL_Keycode key, bool pressed)
        {
            if (KeyPressStates.ContainsKey(key))
                KeyPressStates[key] = pressed;
        }
    }
}
