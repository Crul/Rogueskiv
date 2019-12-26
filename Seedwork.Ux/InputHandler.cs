using Seedwork.Core.Controls;
using Seedwork.Engine;
using System.Collections.Generic;
using System.Linq;
using static SDL2.SDL;

namespace Seedwork.Ux
{
    public class InputHandler<T> : IInputHandler
        where T : IControlable
    {
        private readonly UxContext UxContext;

        private readonly T Game;

        private readonly IGameRenderer GameRenderer;

        private readonly int QuitKey;

        private readonly IDictionary<int, int> KeyControls;

        private readonly IDictionary<int, bool> KeyPressStates;

        public InputHandler(
            UxContext uxContext,
            T game,
            IGameRenderer gameRenderer,
            IDictionary<int, int> keyControls,
            int quitKey
        )
        {
            UxContext = uxContext;
            Game = game;
            GameRenderer = gameRenderer;
            KeyControls = keyControls;
            KeyPressStates = KeyControls
                .ToDictionary(kc => kc.Key, _ => false);
            QuitKey = quitKey;
        }

        public void ProcessEvents()
        {
            while (SDL_PollEvent(out SDL_Event ev) != 0)
                ProcessEvent(ev);

            Game.Controls = KeyPressStates
                .Where(keyState => keyState.Value)
                .Select(keyState => KeyControls[keyState.Key])
                .ToList();
        }

        private void ProcessEvent(SDL_Event ev)
        {
            switch (ev.type)
            {
                case SDL_EventType.SDL_WINDOWEVENT:
                    if (ev.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
                        OnWindowResize(width: ev.window.data1, height: ev.window.data2);

                    return;

                case SDL_EventType.SDL_QUIT:
                    KeyPressStates[QuitKey] = true;
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
            var intKey = (int)key;
            if (KeyPressStates.ContainsKey(intKey))
                KeyPressStates[intKey] = pressed;
        }

        protected virtual void OnWindowResize(int width, int height)
        {
            UxContext.OnWindowResize(width, height);
            GameRenderer.OnWindowResize();
        }

        public void Reset() =>
            KeyPressStates.Keys.ToList().ForEach(k => KeyPressStates[k] = false);
    }
}
