using Rogueskiv.Menus.MenuOptions;
using Seedwork.Core;
using Seedwork.Engine;
using System;
using System.Collections.Generic;

namespace Rogueskiv.Menus
{
    public class RogueskivMenu : Game
    {
        private readonly MenuSys MenuSystem;
        public string CustomSeedText { get => MenuSystem.CustomSeedText; }
        public bool AskingForCustomSeed { get => MenuSystem.AskingForCustomSeed; }

        public RogueskivMenu(
            IRogueskivGameParams gameParams,
            GameStageCode stageCode
        )
            : base(
                quitControl: (int)Menus.Controls.NONE,
                stageCode: stageCode
            )
        {
            MenuSystem = new MenuSys();
            AddSystem(MenuSystem);

            var counter = 0;
            AddEntity(new MenuOptionComp(
                counter++,
                () => "Play Random",
                new Dictionary<Controls, Action>
                {
                    [Menus.Controls.ENTER] = () => EndGame(RogueskivMenuResults.PlayResult)
                }
            ));

            AddEntity(new MenuOptionComp(
                counter++,
                () => "Play Custom Seed",
                new Dictionary<Controls, Action>
                {
                    [Menus.Controls.ENTER] = () => MenuSystem.AskForCustomSeed()
                }
            ));

            AddEntity(new MenuOptionComp(counter++, () => "Settings"));

            AddEntity(new MenuOptionComp(
                counter++,
                () => GetFloorsOptionText(gameParams),
                new Dictionary<Controls, Action>
                {
                    [Menus.Controls.LEFT] = () => gameParams.ChangeFloorCount(-1),
                    [Menus.Controls.RIGHT] = () => gameParams.ChangeFloorCount(+1),
                }
            ));

            AddEntity(new MenuOptionComp(
                counter++,
                () => GetGameModesOptionText(gameParams),
                new Dictionary<Controls, Action>
                {
                    [Menus.Controls.LEFT] = () => gameParams.ChangeGameMode(-1),
                    [Menus.Controls.RIGHT] = () => gameParams.ChangeGameMode(+1),
                }
            ));

            AddEntity(new MenuOptionComp(counter++, () => string.Empty));

            AddEntity(new MenuOptionComp(
                counter++,
                () => "Quit",
                new Dictionary<Controls, Action>
                {
                    [Menus.Controls.ENTER] = () => EndGame(RogueskivMenuResults.QuitResult)
                }
            ));
        }

        public void OnTextInput(string text) => MenuSystem.OnTextInput(text);

        public override void Update()
        {
            Quit = Controls.Contains((int)Menus.Controls.CLOSE_WINDOW);
            base.Update();
        }

        private static string GetFloorsOptionText(IRogueskivGameParams gameParams)
            => GetNumericOptionText(
                title: "Floors   ",
                text: gameParams.FloorCount.ToString(),
                index: gameParams.FloorCount,
                minIndex: gameParams.MinFloorCount,
                maxIndex: gameParams.MaxFloorCount
            );

        private static string GetGameModesOptionText(IRogueskivGameParams gameParams)
            => GetNumericOptionText(
                title: "Game mode",
                text: CleanGameModeText(gameParams.GameMode),
                index: gameParams.GameModeIndex,
                minIndex: 0,
                maxIndex: gameParams.GameModesCount - 1
            );

        private static string CleanGameModeText(string gameMode)
            => gameMode.Substring(gameMode.IndexOf("-") + 1);

        private static string GetNumericOptionText(string title, string text, int index, int minIndex, int maxIndex)
            => $"   {title} " +
               $"{(minIndex < index ? "<" : " ")}" +
               $" {text} " +
               $"{(maxIndex > index ? ">" : " ")}";
    }
}
