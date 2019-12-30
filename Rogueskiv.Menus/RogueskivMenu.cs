using Rogueskiv.Menus.MenuOptions;
using Seedwork.Core;
using Seedwork.Engine;

namespace Rogueskiv.Menus
{
    public class RogueskivMenu : Game
    {
        private readonly MenuSys MenuSystem;
        public string CustomSeedText { get => MenuSystem.CustomSeedText; }
        public bool AskingForCustomSeed { get => MenuSystem.AskingForCustomSeed; }

        public RogueskivMenu(GameStageCode stageCode)
            : base(
                quitControl: (int)Menus.Controls.NONE,
                stageCode: stageCode
            )
        {
            MenuSystem = new MenuSys();
            AddSystem(MenuSystem);

            AddEntity(new MenuOptionComp(0, "Play Random", menuSys => EndGame(RogueskivMenuResults.PlayResult)));
            AddEntity(new MenuOptionComp(1, "Play Custom Seed", menuSys => menuSys.AskForCustomSeed()));
            AddEntity(new MenuOptionComp(2, "Quit", menuSys => EndGame(RogueskivMenuResults.QuitResult)));
        }

        public void OnTextInput(string text) => MenuSystem.OnTextInput(text);
    }
}
