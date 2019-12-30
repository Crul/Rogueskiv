using Seedwork.Core;
using Seedwork.Core.Entities;
using Seedwork.Core.Systems;
using Seedwork.Crosscutting;
using System.Collections.Generic;
using System.Linq;
using static SDL2.SDL;

namespace Rogueskiv.Menus.MenuOptions
{
    class MenuSys : BaseSystem
    {
        private int CustomSeed;
        public string CustomSeedText
        {
            get => CustomSeed.ToString();

            private set
            {
                if (string.IsNullOrEmpty(value))
                    CustomSeed = 0;

                else
                {
                    var backup = CustomSeed;
                    if (!int.TryParse(value, out CustomSeed))
                        CustomSeed = backup;
                }
            }
        }

        public bool AskingForCustomSeed { get; private set; } = false;

        private Game Game;
        private List<Controls> LastControls = new List<Controls>();

        public override void Init(Game game) => Game = game;

        public override void Update(EntityList entities, List<int> controls)
        {
            if (controls.Count == 0)
            {
                LastControls.Clear();
                return;
            }

            if (AskingForCustomSeed)
                UpdateCustomSeedInput(controls);
            else
                UpdateMenuOptions(entities, controls);

            LastControls = controls.Select(c => (Controls)c).ToList();
        }

        internal void OnTextInput(string text)
        {
            if (AskingForCustomSeed)
                CustomSeedText += text;
        }

        private void UpdateMenuOptions(EntityList entities, List<int> controls)
        {
            var menuOptions = entities.GetComponents<MenuOptionComp>()
                .OrderBy(mo => mo.Order)
                .ToList();

            var activeMenuOption = menuOptions.Single(mo => mo.Active);
            if (ControlPressed(controls, Controls.ENTER))
            {
                activeMenuOption.Execute(this);
                return;
            }

            int newActiveIndex;
            if (ControlPressed(controls, Controls.QUIT))
                newActiveIndex = menuOptions.Count - 1;

            else
            {
                var move = 0;
                if (ControlPressed(controls, Controls.UP))
                    move -= 1;
                if (ControlPressed(controls, Controls.DOWN))
                    move += 1;

                if (move == 0)
                    return;

                newActiveIndex = Maths.Modulo(
                    menuOptions.IndexOf(activeMenuOption) + move,
                    menuOptions.Count
                );
            }

            activeMenuOption.Active = false;
            menuOptions[newActiveIndex].Active = true;
        }

        private void UpdateCustomSeedInput(List<int> controls)
        {
            if (ControlPressed(controls, Controls.ENTER))
            {
                RogueskivMenuResults.PlayResult.GameSeed = CustomSeed;

                Game.EndGame(RogueskivMenuResults.PlayResult);
                return;
            }

            if (ControlPressed(controls, Controls.QUIT))
            {
                CustomSeed = 0;
                AskingForCustomSeed = false;
                SDL_StopTextInput();
                return;
            }

            if (ControlPressed(controls, Controls.COPY))
            {
                SDL_SetClipboardText(CustomSeedText);
                return;
            }

            if (ControlPressed(controls, Controls.PASTE))
            {
                CustomSeedText = SDL_GetClipboardText();
                return;
            }

            if (ControlPressed(controls, Controls.BACKSPACE))
            {
                if (!string.IsNullOrEmpty(CustomSeedText))
                    CustomSeedText = CustomSeedText[0..^1];

                return;
            }
        }

        public void AskForCustomSeed()
        {
            AskingForCustomSeed = true;
            SDL_StartTextInput();
        }

        private bool ControlPressed(List<int> controls, Controls control) =>
            controls.Contains((int)control) && !LastControls.Contains(control);
    }
}
