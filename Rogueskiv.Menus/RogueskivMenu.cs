using Rogueskiv.Menus.MenuOptions;
using Seedwork.Core;
using Seedwork.Core.Components;
using Seedwork.Core.Systems;
using Seedwork.Engine;
using System.Collections.Generic;

namespace Rogueskiv.Menus
{
    public class RogueskivMenu : Game
    {
        public RogueskivMenu(GameStageCode stageCode)
            : base(
                quitControl: (int)Menus.Controls.QUIT,
                stageCode: stageCode,
                entitiesComponents: new List<List<IComponent>>
                {
                    new List<IComponent> { new MenuOptionComp(0, "Play", RogueskivMenuResults.PlayResult) },
                    new List<IComponent> { new MenuOptionComp(1, "Quit", RogueskivMenuResults.QuitResult) },
                },
                systems: new List<ISystem> { new MenuSys() }
            )
        { }
    }
}
