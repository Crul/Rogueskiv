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

        private Game Game;
        private StatsComp StatsComp;
        private List<Controls> LastControls = new List<Controls>();

        private enum MenuStates { MainMenu, CustomSeedInput, StatsView };
        private MenuStates MenuState;
        internal bool IsMainMenuView => MenuState == MenuStates.MainMenu;
        internal bool IsCustomSeedInput => MenuState == MenuStates.CustomSeedInput;

        public override void Init(Game game)
        {
            Game = game;
            StatsComp = game.Entities.GetSingleComponent<StatsComp>();
        }

        public override void Update(EntityList entities, List<int> controls)
        {
            LastControls.Clear();
            if (controls.Count == 0)
                return;

            switch (MenuState)
            {
                case MenuStates.MainMenu:
                    UpdateMenuOptions(entities, controls);
                    break;
                case MenuStates.CustomSeedInput:
                    UpdateCustomSeedInput(controls);
                    break;
                case MenuStates.StatsView:
                    UpdateStatsView(controls);
                    break;
                default:
                    break;
            }

            LastControls = controls.Select(c => (Controls)c).ToList();
        }

        internal void OnTextInput(string text)
        {
            if (MenuState == MenuStates.CustomSeedInput)
                CustomSeedText += text;
        }

        internal void ShowStats()
        {
            MenuState = MenuStates.StatsView;
            StatsComp.Show();
        }

        private void UpdateMenuOptions(EntityList entities, List<int> controls)
        {
            var menuOptions = entities.GetComponents<MenuOptionComp>()
                .OrderBy(mo => mo.Order)
                .ToList();

            var activeMenuOption = menuOptions.Single(mo => mo.Active);

            var actionsToExecute = activeMenuOption
                .ActionsByControl
                .Keys
                .Where(control => ControlPressed(controls, control))
                .Select(control => activeMenuOption.ActionsByControl[control])
                .ToList();

            actionsToExecute.ForEach(action => action());

            if (actionsToExecute.Any())
                return;

            int? indexToActivate = null;
            if (ControlPressed(controls, Controls.ENTER))
                indexToActivate = 0;

            if (ControlPressed(controls, Controls.QUIT))
                indexToActivate = menuOptions.Count - 1;

            var move = 0;
            if (ControlPressed(controls, Controls.UP))
                move -= 1;
            if (ControlPressed(controls, Controls.DOWN))
                move += 1;

            if (move != 0)
            {
                indexToActivate = menuOptions.IndexOf(activeMenuOption);
                do
                {
                    indexToActivate += move;
                    indexToActivate = Maths.Modulo(indexToActivate.Value, menuOptions.Count);
                }
                while (!menuOptions[indexToActivate.Value].Focusable);
            }

            if (indexToActivate.HasValue)
            {
                activeMenuOption.Active = false;
                menuOptions[indexToActivate.Value].Active = true;
            }
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
                MenuState = MenuStates.MainMenu;
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

        private void UpdateStatsView(List<int> controls)
        {
            if (ControlPressed(controls, Controls.QUIT) || ControlPressed(controls, Controls.ENTER))
            {
                StatsComp.Hide();
                MenuState = MenuStates.MainMenu;
                return;
            }

            if (ControlPressed(controls, Controls.UP))
                StatsComp.Page--;

            if (ControlPressed(controls, Controls.DOWN))
                StatsComp.Page++;
        }

        public void AskForCustomSeed()
        {
            MenuState = MenuStates.CustomSeedInput;
            SDL_StartTextInput();
        }

        private bool ControlPressed(List<int> controls, Controls control) =>
            controls.Contains((int)control) && !LastControls.Contains(control);
    }
}
