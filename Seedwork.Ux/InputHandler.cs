using Seedwork.Core.Controls;
using Seedwork.Engine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SDL2.SDL;

namespace Seedwork.Ux
{
    public class InputHandler<T> : IInputHandler
        where T : IControlable
    {
        private readonly UxContext UxContext;

        private readonly T Game;

        private readonly IGameRenderer GameRenderer;

        private readonly IDictionary<int, int> ControlsByKeys;

        protected readonly IDictionary<int, bool> ControlStates;

        private readonly bool AllowRepeats;

        private readonly int CloseWindowControl;
        private bool CloseWindowKeyPressed;

        private readonly int ToggleMusicControl;
        private bool ToggleMusicKeyPressedLastTime;

        public InputHandler(
            UxContext uxContext,
            T game,
            IGameRenderer gameRenderer,
            IDictionary<int, int> controlsByKeys,
            int closeWindowControl,
            int toggleMusicControl,
            bool allowRepeats = false
        )
        {
            UxContext = uxContext;
            Game = game;
            GameRenderer = gameRenderer;
            ControlsByKeys = controlsByKeys;
            ControlStates = ControlsByKeys
                .Select(kc => kc.Value)
                .Distinct()
                .ToDictionary(kc => kc, _ => false);

            CloseWindowControl = closeWindowControl;
            ToggleMusicControl = toggleMusicControl;
            AllowRepeats = allowRepeats;
        }

        public virtual void ProcessEvents()
        {
            while (SDL_PollEvent(out SDL_Event ev) != 0)
                ProcessEvent(ev);

            Game.Controls = ControlStates
                .Where(controlState => controlState.Value)
                .Select(controlState => controlState.Key)
                .ToList();

            if (CloseWindowKeyPressed)
            {
                Game.Controls.Add(CloseWindowControl);
                CloseWindowKeyPressed = false;
            }
        }

        private void ProcessEvent(SDL_Event ev)
        {
            switch (ev.type)
            {
                case SDL_EventType.SDL_WINDOWEVENT:
                    if (ev.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
                        UxContext.OnWindowResize(width: ev.window.data1, height: ev.window.data2);
                    return;

                case SDL_EventType.SDL_QUIT:
                    CloseWindowKeyPressed = true;
                    return;

                case SDL_EventType.SDL_KEYDOWN:
                    OnKeyEvent(ev.key, true);
                    return;

                case SDL_EventType.SDL_KEYUP:
                    OnKeyEvent(ev.key, false);
                    return;

                case SDL_EventType.SDL_TEXTINPUT:
                    OnTextInput(GetText(ev.text));
                    return;

                case SDL_EventType.SDL_RENDER_TARGETS_RESET:
                    GameRenderer.RecreateTextures();
                    return;
            }
        }

        private void OnKeyEvent(SDL_KeyboardEvent keyEvent, bool pressed)
            => OnKeyEvent(keyEvent.keysym.sym, pressed, keyEvent.repeat != 0);

        protected virtual void OnKeyEvent(SDL_Keycode key, bool pressed, bool isRepeat)
        {
            var intKey = (int)key;
            if (ControlsByKeys.ContainsKey(intKey))
            {
                if (AllowRepeats || !isRepeat)
                    ControlStates[ControlsByKeys[intKey]] = pressed;

                HandleToggleMusic();
            }
        }

        protected virtual void OnTextInput(string text) { }

        private void HandleToggleMusic()
        {
            var isPressedMusicControl = ControlStates[ToggleMusicControl];
            if (!ToggleMusicKeyPressedLastTime && isPressedMusicControl)
                UxContext.ToggleMusic();

            ToggleMusicKeyPressedLastTime = isPressedMusicControl;
        }

        private static string GetText(SDL_TextInputEvent textEvent)
        {
            unsafe
            {
                var i = 0;
                var data = new byte[SDL_TEXTINPUTEVENT_TEXT_SIZE];
                for (; i < SDL_TEXTINPUTEVENT_TEXT_SIZE; i++)
                {
                    var b = textEvent.text[i];
                    if (b == '\0')
                        break;

                    data[i] = b;
                }

                return Encoding.UTF8.GetString(data, 0, i);
            }
        }

        public void Reset() =>
            ControlStates.Keys.ToList().ForEach(k => ControlStates[k] = false);

        public void SetControls(IInputHandler inputHandler)
            => ControlStates.Keys.ToList().ForEach(k => ControlStates[k] = inputHandler.GetControlState(k));

        public bool GetControlState(int control) => ControlStates[control];
    }
}
