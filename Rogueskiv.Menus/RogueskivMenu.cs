using Rogueskiv.Menus.MenuOptions;
using Seedwork.Core;
using Seedwork.Core.Systems;
using Seedwork.Engine;
using System.Collections.Generic;

namespace Rogueskiv.Menus
{
    public class RogueskivMenu : Game
    {
        public RogueskivMenu(GameStageCode stageCode)
            : base(
                quitControl: (int)Menus.Controls.NONE,
                stageCode: stageCode,
                systems: new List<ISystem> { new MenuSys() }
            )
        {
            AddEntity(new MenuOptionComp(0, "Play", menuSys => EndGame(RogueskivMenuResults.PlayResult)));
            AddEntity(new MenuOptionComp(1, "Quit", menuSys => EndGame(RogueskivMenuResults.QuitResult)));
        }
    }
}
